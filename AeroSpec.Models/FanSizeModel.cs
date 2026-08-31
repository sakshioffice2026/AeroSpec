namespace AeroSpec.Models;

public class FanSizeModel
{
    public int Id { get; set; }
    public string SizeId { get; set; } = string.Empty;
    public double DiameterIn { get; set; }
    public double CfmScale { get; set; }
    public double SpScale { get; set; }
    public int MaxRpm { get; set; }
    public double OutletArea { get; set; }
    public int WeightBase { get; set; }
    public int BasePrice { get; set; }
}