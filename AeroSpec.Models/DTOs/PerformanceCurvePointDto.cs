namespace AeroSpec.Models.DTOs;

public class PerformanceCurvePointDto
{
    public double Cfm { get; set; }
    public double FanSp { get; set; }
    public double Eff { get; set; }
    public double SysSp { get; set; }
}

public class GhostCurvePointDto
{
    public double Cfm { get; set; }
    public double Sp { get; set; }
}