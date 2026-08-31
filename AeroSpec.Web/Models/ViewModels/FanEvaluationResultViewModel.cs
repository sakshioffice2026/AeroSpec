namespace AeroSpec.Web.ViewModels;

public class FanEvaluationResultViewModel
{
    public string SizeId { get; set; } = string.Empty;
    public double DiameterIn { get; set; }
    public bool Feasible { get; set; }
    public string? Reason { get; set; }
    public int Rpm { get; set; }
    public int EffPct { get; set; }
    public double Bhp { get; set; }
    public int MotorHp { get; set; }
    public int TipSpeed { get; set; }
    public int OutletVelocity { get; set; }
    public int SoundDba { get; set; }
    public bool IsSelected { get; set; }
}
