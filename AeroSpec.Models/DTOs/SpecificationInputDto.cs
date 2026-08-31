namespace AeroSpec.Models.DTOs;

public class SpecificationInputDto
{
    public double Cfm { get; set; }
    public double Sp { get; set; }
    public int Elevation { get; set; }
    public int Temperature { get; set; }
    public string FanType { get; set; } = "housed_belt";
    public string Arrangement { get; set; } = "Arrangement 9 – Belt Drive";
    public int MaxTipSpeed { get; set; } = 16000;
    public int? SoundLimit { get; set; }
    public int Qty { get; set; } = 1;
    public string ProjectName { get; set; } = string.Empty;
    public string? Tag { get; set; }
}