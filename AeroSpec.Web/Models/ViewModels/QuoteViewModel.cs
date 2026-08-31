namespace AeroSpec.Web.ViewModels;

public class QuoteViewModel
{
    public string ProjectName { get; set; } = string.Empty;
    public string? Tag { get; set; }
    public string SelectedSizeId { get; set; } = string.Empty;
    public string FanTypeLabel { get; set; } = string.Empty;
    public string Arrangement { get; set; } = string.Empty;
    public int UnitPrice { get; set; }
    public int Qty { get; set; }
    public int Subtotal { get; set; }
    public int Freight { get; set; }
    public int Total { get; set; }
    public DateTime QuoteDate { get; set; }
}
