using AeroSpec.Database;

namespace AeroSpec.Models.DTOs;

public class QuoteDto
{
    public FanEvaluationResultDto SelectedFan { get; set; } = new();
    public SpecificationInputDto Specification { get; set; } = new();
    public FanType FanTypeInfo { get; set; } = new();

    public int UnitPrice { get; set; }
    public int Qty { get; set; }
    public int Subtotal { get; set; }
    public int Freight { get; set; }
    public int Total { get; set; }

    public DateTime QuoteDate { get; set; } = DateTime.Now;
}