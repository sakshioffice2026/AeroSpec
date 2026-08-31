namespace AeroSpec.Models.DTOs;

public class FanSelectionResultDto
{
    public int SavedSelectionId { get; set; }
    public SpecificationInputDto Specification { get; set; } = new();
    public FanEvaluationResultDto SelectedFan { get; set; } = new();
    public List<FanEvaluationResultDto> AllResults { get; set; } = new();
    public PerformanceCurveDto Curve { get; set; } = new();
    public QuoteDto Quote { get; set; } = new();
    public double DensityRatio { get; set; }
    public bool HasFeasibleFan { get; set; }
    public string? Message { get; set; }
}