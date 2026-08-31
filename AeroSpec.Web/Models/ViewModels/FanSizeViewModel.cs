using System.ComponentModel.DataAnnotations;

namespace AeroSpec.Web.ViewModels;

public class FanSizeViewModel
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    [Display(Name = "Size ID")]
    public string SizeId { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Diameter (in)")]
    public double DiameterIn { get; set; }

    [Required]
    [Display(Name = "CFM Scale")]
    public double CfmScale { get; set; }

    [Required]
    [Display(Name = "SP Scale")]
    public double SpScale { get; set; }

    [Required]
    [Display(Name = "Max RPM")]
    public int MaxRpm { get; set; }

    [Required]
    [Display(Name = "Outlet Area (sq ft)")]
    public double OutletArea { get; set; }

    [Required]
    [Display(Name = "Base Weight (lb)")]
    public int WeightBase { get; set; }

    [Required]
    [Display(Name = "Base Price ($)")]
    public int BasePrice { get; set; }
}
