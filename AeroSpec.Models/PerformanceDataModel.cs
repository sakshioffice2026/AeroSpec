namespace AeroSpec.Models;

public class PerformanceDataModel
{
    public int Id { get; set; }
    public int FanSizeId { get; set; }
    public int Rpm { get; set; }
    public double Volume { get; set; }
    public double StaticPressure { get; set; }
}