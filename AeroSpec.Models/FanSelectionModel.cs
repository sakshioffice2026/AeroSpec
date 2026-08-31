namespace AeroSpec.Models;

public class FanSelectionModel
{
    public int Id { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string? Tag { get; set; }
    public double RequiredCfm { get; set; }
    public double RequiredSp { get; set; }
    public int Elevation { get; set; }
    public int Temperature { get; set; }
    public string FanType { get; set; } = string.Empty;
    public string Arrangement { get; set; } = string.Empty;
    public int MaxTipSpeed { get; set; }
    public int? SoundLimit { get; set; }
    public int Quantity { get; set; }
    public string SelectedFanId { get; set; } = string.Empty;
    public int SelectedRpm { get; set; }
    public double SelectedBhp { get; set; }
    public int SelectedMotorHp { get; set; }
    public double SelectedEfficiency { get; set; }
    public int SelectedTipSpeed { get; set; }
    public int SelectedSound { get; set; }
    public double DensityRatio { get; set; }
    public DateTime CreatedDate { get; set; }
}