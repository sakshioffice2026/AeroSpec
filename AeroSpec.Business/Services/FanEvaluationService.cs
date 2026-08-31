using AeroSpec.Business.Contracts;
using AeroSpec.Database;
using AeroSpec.Models.DTOs;
using AeroSpec.Repositories.Contracts;


namespace AeroSpec.Business.Services;

public class FanEvaluationService : IFanEvaluationService
{
    private readonly IFanSizeRepository _fanSizeRepository;
    private readonly IPerformanceDataRepository _performanceDataRepository;
    private readonly IFanCalculationService _calculationService;
    private readonly double[] _baseQ = { 0, 380, 760, 1140, 1520, 1900, 2280, 2660, 3040, 3420, 3800, 4180, 4500 };
    private readonly double[] _baseSp = { 4.10, 4.28, 4.35, 4.30, 4.12, 3.80, 3.35, 2.78, 2.10, 1.38, 0.70, 0.22, 0.00 };
    private readonly double[] _baseEff = { 0.02, 0.34, 0.54, 0.67, 0.75, 0.80, 0.83, 0.81, 0.76, 0.65, 0.50, 0.30, 0.02 };

    public FanEvaluationService(
        IFanSizeRepository fanSizeRepository,
        IPerformanceDataRepository performanceDataRepository,
        IFanCalculationService calculationService)
    {
        _fanSizeRepository = fanSizeRepository ?? throw new ArgumentNullException(nameof(fanSizeRepository));
        _performanceDataRepository = performanceDataRepository ?? throw new ArgumentNullException(nameof(performanceDataRepository));
        _calculationService = calculationService ?? throw new ArgumentNullException(nameof(calculationService));
    }

    public async Task<List<FanEvaluationResultDto>> EvaluateAllFanSizesAsync(
        SpecificationInputDto input,
        FanType fanType,
        double densityRatio)
    {
        var fanSizes = await _fanSizeRepository.GetAllActiveAsync();
        var results = new List<FanEvaluationResultDto>();

        foreach (var size in fanSizes)
        {
            var result = await EvaluateSizeAsync(size, input, fanType, densityRatio);
            results.Add(result);
        }

        return results.OrderByDescending(x => x.EffPct).ToList();
    }

    private async Task<FanEvaluationResultDto> EvaluateSizeAsync(
        FanSize size,
        SpecificationInputDto input,
        FanType fanType,
        double densityRatio)
    {
        var cfmReq = input.Cfm;
        var spReqActual = input.Sp;
        var spReqStd = spReqActual / densityRatio;
        var spScaleAdj = size.SpScale * fanType.SpMod;

        var t = _calculationService.FindDutyT(cfmReq, spReqStd, size.CfmScale, spScaleAdj);

        if (t == null)
        {
            return new FanEvaluationResultDto
            {
                Size = size,
                Feasible = false,
                Reason = "Duty point falls outside this size's performance envelope"
            };
        }

        var qBase = _calculationService.InterpolateArray(_baseQ, (double)t) * size.CfmScale;
        var N = (1000 * cfmReq) / qBase;
        var effBase = _calculationService.InterpolateArray(_baseEff, (double)t) * fanType.EffMod;
        var eff = Math.Min(0.89, effBase);
        var tipSpeed = Math.PI * (size.DiameterIn / 12) * N;
        var bhpStd = (cfmReq * spReqStd) / (6356 * Math.Max(eff, 0.08));
        var beltLossFactor = input.Arrangement.Contains("Belt") ? 1.03 : 1.0;
        var bhpActual = bhpStd * densityRatio * beltLossFactor;
        var outletVelocity = cfmReq / size.OutletArea;

        var rpmOk = N <= size.MaxRpm && N >= 120;
        var tipOk = tipSpeed <= input.MaxTipSpeed;
        var feasible = rpmOk && tipOk;

        var reasons = new List<string>();
        if (!rpmOk)
            reasons.Add(N > size.MaxRpm ? "Exceeds maximum RPM for this size" : "Below minimum stable RPM");
        if (!tipOk)
            reasons.Add("Exceeds specified max tip speed limit");

        var motorHp = _calculationService.SelectMotorHp(bhpActual);
        var soundDba = Math.Max(62, Math.Min(97, 41 + 12 * Math.Log10(bhpActual + 1) + (tipSpeed / 1000) * 1.9));

        return new FanEvaluationResultDto
        {
            Size = size,
            Feasible = feasible,
            Reason = string.Join("; ", reasons),
            T = (double)t,
            N = (int)Math.Round(N),
            Eff = eff,
            EffPct = (int)Math.Round(eff * 100),
            Bhp = bhpActual,
            MotorHp = motorHp,
            TipSpeed = (int)Math.Round(tipSpeed),
            OutletVelocity = (int)Math.Round(outletVelocity),
            SoundDba = (int)Math.Round(soundDba)
        };
    }

    public async Task<PerformanceCurveDto> BuildCurveDataAsync(
        FanEvaluationResultDto evalResult,
        SpecificationInputDto input,
        double densityRatio,
        FanType fanType)
    {
        var size = evalResult.Size;
        var N = evalResult.N;
        var spScaleAdj = size.SpScale * fanType.SpMod;
        var points = new List<PerformanceCurvePointDto>();
        var k = input.Sp / (input.Cfm * input.Cfm);

        for (var i = 0; i <= 24; i++)
        {
            var t = (i / 24.0) * 11.999;
            var cfm = _calculationService.InterpolateArray(_baseQ, t) * size.CfmScale * (N / 1000.0);
            var spStd = _calculationService.InterpolateArray(_baseSp, t) * spScaleAdj * Math.Pow(N / 1000.0, 2);
            var spActual = spStd * densityRatio;
            var eff = Math.Min(0.89, _calculationService.InterpolateArray(_baseEff, t) * fanType.EffMod) * 100;

            points.Add(new PerformanceCurvePointDto
            {
                Cfm = cfm,
                FanSp = spActual,
                Eff = eff,
                SysSp = k * cfm * cfm
            });
        }

        var ghostLow = BuildGhostCurve(size, N, spScaleAdj, densityRatio, 0.82);
        var ghostHigh = BuildGhostCurve(size, N, spScaleAdj, densityRatio, 1.18);

        return new PerformanceCurveDto
        {
            Points = points,
            GhostLow = ghostLow,
            GhostHigh = ghostHigh
        };
    }

    private List<GhostCurvePointDto> BuildGhostCurve(
        FanSize size,
        int baseN,
        double spScaleAdj,
        double densityRatio,
        double multiplier)
    {
        var ghostPoints = new List<GhostCurvePointDto>();
        var Ng = Math.Min(size.MaxRpm, baseN * multiplier);

        for (var i = 0; i <= 12; i++)
        {
            var t = (i / 12.0) * 11.999;
            var cfm = _calculationService.InterpolateArray(_baseQ, t) * size.CfmScale * (Ng / 1000);
            var spStd = _calculationService.InterpolateArray(_baseSp, t) * spScaleAdj * Math.Pow(Ng / 1000, 2);

            ghostPoints.Add(new GhostCurvePointDto
            {
                Cfm = cfm,
                Sp = spStd * densityRatio
            });
        }

        return ghostPoints;
    }
}