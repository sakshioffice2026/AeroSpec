namespace AeroSpec.Web.ViewModels;

public class FanSelectionHistoryViewModel
{
    public int Id { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string? Tag { get; set; }
    public double RequiredCfm { get; set; }
    public double RequiredSp { get; set; }
    public string FanType { get; set; } = string.Empty;
    public string SelectedFanId { get; set; } = string.Empty;
    public int SelectedRpm { get; set; }
    public int SelectedMotorHp { get; set; }
    public double SelectedEfficiency { get; set; }
    public int Quantity { get; set; }
    public DateTime CreatedDate { get; set; }
}
