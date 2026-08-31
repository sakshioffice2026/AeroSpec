namespace AeroSpec.Business.Contracts;

public interface IFanCalculationService
{
    double ComputeDensityRatio(int elevationFt, int tempF);
    double InterpolateArray(double[] arr, double t);
    double? FindDutyT(double cfmReq, double spReqStd, double cfmScale, double spScale);
    int SelectMotorHp(double bhp);
    string ClassForTipSpeed(int tipSpeed);
}