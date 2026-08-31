namespace AeroSpec.Web.ViewModels;

public class FanSelectionResultViewModel
{
    public int SavedSelectionId { get; set; }
    public SpecificationInputViewModel Specification { get; set; } = new();
    public bool HasFeasibleFan { get; set; }
    public string? Message { get; set; }
    public double DensityRatio { get; set; }
    public FanEvaluationResultViewModel? SelectedFan { get; set; }
    public List<FanEvaluationResultViewModel> AllResults { get; set; } = new();
    public PerformanceCurveViewModel Curve { get; set; } = new();
    public QuoteViewModel Quote { get; set; } = new();
}
