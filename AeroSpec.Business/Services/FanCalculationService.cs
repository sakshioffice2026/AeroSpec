using AeroSpec.Business.Contracts;

namespace AeroSpec.Business.Services;

public class FanCalculationService : IFanCalculationService
{
    private readonly double[] _baseQ = { 0, 380, 760, 1140, 1520, 1900, 2280, 2660, 3040, 3420, 3800, 4180, 4500 };
    private readonly double[] _baseSp = { 4.10, 4.28, 4.35, 4.30, 4.12, 3.80, 3.35, 2.78, 2.10, 1.38, 0.70, 0.22, 0.00 };
    private readonly double[] _baseEff = { 0.02, 0.34, 0.54, 0.67, 0.75, 0.80, 0.83, 0.81, 0.76, 0.65, 0.50, 0.30, 0.02 };
    private readonly int[] _standardMotorHp = { 0, 1, 2, 3, 5, 7, 10, 15, 20, 30, 40, 50, 75, 100, 150, 200 };

    public double ComputeDensityRatio(int elevationFt, int tempF)
    {
        var altFactor = Math.Exp(-elevationFt / 26000.0);
        var tempFactor = 530.0 / (tempF + 460.0);
        return altFactor * tempFactor;
    }

    public double InterpolateArray(double[] arr, double t)
    {
        var i0 = Math.Max(0, Math.Min(11, (int)Math.Floor(t)));
        var i1 = i0 + 1;
        var f = t - i0;
        return arr[i0] + (arr[i1] - arr[i0]) * f;
    }

    public double? FindDutyT(double cfmReq, double spReqStd, double cfmScale, double spScale)
    {
        double? prevG = null;
        double? prevT = null;
        var steps = 1500;

        for (var i = 1; i <= steps; i++)
        {
            var t = (i / (double)steps) * 11.999;
            var q = InterpolateArray(_baseQ, t) * cfmScale;

            if (q <= 1)
            {
                prevT = t;
                prevG = null;
                continue;
            }

            var sp = InterpolateArray(_baseSp, t) * spScale;
            var g = spReqStd * q * q - sp * cfmReq * cfmReq;

            if (prevG != null && Math.Sign((double)prevG) != Math.Sign(g) && prevG != 0)
            {
                return prevT + (t - (double)prevT) * (0 - (double)prevG) / (g - (double)prevG);
            }

            prevG = g;
            prevT = t;
        }

        return null;
    }

    public int SelectMotorHp(double bhp)
    {
        var target = bhp * 1.15;

        foreach (var hp in _standardMotorHp)
        {
            if (hp >= target)
                return hp;
        }

        return _standardMotorHp[_standardMotorHp.Length - 1];
    }

    public string ClassForTipSpeed(int tipSpeed)
    {
        if (tipSpeed <= 12000) return "AMCA Class I";
        if (tipSpeed <= 16000) return "AMCA Class II";
        return "AMCA Class III";
    }
}