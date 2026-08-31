

using AeroSpec.Database;

namespace AeroSpec.Models.DTOs;

public class FanEvaluationResultDto
{
    public FanSize Size { get; set; } = new();
    public bool Feasible { get; set; }
    public string? Reason { get; set; }
    public double T { get; set; }
    public int N { get; set; }
    public double Eff { get; set; }
    public int EffPct { get; set; }
    public double Bhp { get; set; }
    public int MotorHp { get; set; }
    public int TipSpeed { get; set; }
    public int OutletVelocity { get; set; }
    public int SoundDba { get; set; }
}