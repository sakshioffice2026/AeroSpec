using AeroSpec.Business.Contracts;
using AeroSpec.Database;
using AeroSpec.Models.DTOs;
using AeroSpec.Repositories.Contracts;

namespace AeroSpec.Business.Services;

public class FanSelectionService : IFanSelectionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFanCalculationService _calculationService;
    private readonly IFanEvaluationService _evaluationService;
    private readonly IQuoteService _quoteService;

    public FanSelectionService(
        IUnitOfWork unitOfWork,
        IFanCalculationService calculationService,
        IFanEvaluationService evaluationService,
        IQuoteService quoteService)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _calculationService = calculationService ?? throw new ArgumentNullException(nameof(calculationService));
        _evaluationService = evaluationService ?? throw new ArgumentNullException(nameof(evaluationService));
        _quoteService = quoteService ?? throw new ArgumentNullException(nameof(quoteService));
    }

    public async Task<FanSelectionResultDto> ProcessSelectionAsync(SpecificationInputDto input)
    {
        if (input == null) throw new ArgumentNullException(nameof(input));

        var fanType = await _unitOfWork.FanTypeRepository.GetByTypeIdAsync(input.FanType);
        if (fanType == null)
        {
            return new FanSelectionResultDto
            {
                Specification = input,
                HasFeasibleFan = false,
                Message = $"Unknown fan type '{input.FanType}'."
            };
        }

        var densityRatio = _calculationService.ComputeDensityRatio(input.Elevation, input.Temperature);

        var allResults = await _evaluationService.EvaluateAllFanSizesAsync(input, fanType, densityRatio);

        var feasibleResults = allResults.Where(r => r.Feasible).ToList();

        if (feasibleResults.Count == 0)
        {
            return new FanSelectionResultDto
            {
                Specification = input,
                AllResults = allResults,
                DensityRatio = densityRatio,
                HasFeasibleFan = false,
                Message = "No fan size satisfies the required duty point within the given constraints."
            };
        }

        if (input.SoundLimit.HasValue)
        {
            var withinSound = feasibleResults.Where(r => r.SoundDba <= input.SoundLimit.Value).ToList();
            if (withinSound.Count > 0)
            {
                feasibleResults = withinSound;
            }
        }

        var selectedFan = feasibleResults
            .OrderByDescending(r => r.EffPct)
            .ThenBy(r => r.Size.DiameterIn)
            .First();

        var curve = await _evaluationService.BuildCurveDataAsync(selectedFan, input, densityRatio, fanType);
        var quote = await _quoteService.GenerateQuoteAsync(selectedFan, input, fanType);

        var entity = new FanSelection
        {
            ProjectName = input.ProjectName,
            Tag = input.Tag,
            RequiredCfm = input.Cfm,
            RequiredSp = input.Sp,
            Elevation = input.Elevation,
            Temperature = input.Temperature,
            FanType = input.FanType,
            Arrangement = input.Arrangement,
            MaxTipSpeed = input.MaxTipSpeed,
            SoundLimit = input.SoundLimit,
            Quantity = input.Qty,
            SelectedFanId = selectedFan.Size.SizeId,
            SelectedRpm = selectedFan.N,
            SelectedBhp = selectedFan.Bhp,
            SelectedMotorHp = selectedFan.MotorHp,
            SelectedEfficiency = selectedFan.Eff,
            SelectedTipSpeed = selectedFan.TipSpeed,
            SelectedSound = selectedFan.SoundDba,
            DensityRatio = densityRatio
        };

        var saved = await _unitOfWork.FanSelectionRepository.AddAsync(entity);

        return new FanSelectionResultDto
        {
            SavedSelectionId = saved.Id,
            Specification = input,
            SelectedFan = selectedFan,
            AllResults = allResults,
            Curve = curve,
            Quote = quote,
            DensityRatio = densityRatio,
            HasFeasibleFan = true
        };
    }

    public async Task<FanSelection?> GetByIdAsync(int id)
    {
        return await _unitOfWork.FanSelectionRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<FanSelection>> GetHistoryAsync()
    {
        return await _unitOfWork.FanSelectionRepository.GetAllActiveAsync();
    }

    public async Task<IEnumerable<FanSelection>> GetByProjectNameAsync(string projectName)
    {
        return await _unitOfWork.FanSelectionRepository.GetByProjectNameAsync(projectName);
    }

    public async Task DeleteAsync(int id)
    {
        await _unitOfWork.FanSelectionRepository.DeleteAsync(id);
    }
}