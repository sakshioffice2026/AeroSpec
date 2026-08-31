namespace AeroSpec.Models;

public class FanTypeModel
{
    public int Id { get; set; }
    public string TypeId { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Prefix { get; set; } = string.Empty;
    public double SpMod { get; set; }
    public double EffMod { get; set; }
}