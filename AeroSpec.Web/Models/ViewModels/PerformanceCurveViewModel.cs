namespace AeroSpec.Web.ViewModels;

public class PerformanceCurveViewModel
{
    public string SelectedSizeId { get; set; } = string.Empty;
    public List<CurvePointViewModel> Points { get; set; } = new();
    public List<GhostPointViewModel> GhostLow { get; set; } = new();
    public List<GhostPointViewModel> GhostHigh { get; set; } = new();
    public double DutyCfm { get; set; }
    public double DutySp { get; set; }
}

public class CurvePointViewModel
{
    public double Cfm { get; set; }
    public double FanSp { get; set; }
    public double Eff { get; set; }
    public double SysSp { get; set; }
}

public class GhostPointViewModel
{
    public double Cfm { get; set; }
    public double Sp { get; set; }
}
