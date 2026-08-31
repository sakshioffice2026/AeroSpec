namespace AeroSpec.Models.DTOs;

public class PerformanceCurveDto
{
    public List<PerformanceCurvePointDto> Points { get; set; } = new();
    public List<GhostCurvePointDto> GhostLow { get; set; } = new();
    public List<GhostCurvePointDto> GhostHigh { get; set; } = new();
}