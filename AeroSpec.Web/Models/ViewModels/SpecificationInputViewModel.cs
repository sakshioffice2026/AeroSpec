using System.ComponentModel.DataAnnotations;

namespace AeroSpec.Web.ViewModels;

public class SpecificationInputViewModel
{
    [Required]
    [Display(Name = "Project Name")]
    [StringLength(255)]
    public string ProjectName { get; set; } = string.Empty;

    [StringLength(100)]
    public string? Tag { get; set; }

    [Required]
    [Range(1, 1_000_000)]
    [Display(Name = "Required CFM")]
    public double Cfm { get; set; }

    [Required]
    [Range(0.01, 100)]
    [Display(Name = "Required Static Pressure (in. wg)")]
    public double Sp { get; set; }

    [Range(0, 15000)]
    [Display(Name = "Elevation (ft)")]
    public int Elevation { get; set; }

    [Range(-50, 500)]
    [Display(Name = "Temperature (°F)")]
    public int Temperature { get; set; } = 70;

    [Required]
    [Display(Name = "Fan Type")]
    public string FanType { get; set; } = "housed_belt";

    [Required]
    [Display(Name = "Arrangement")]
    public string Arrangement { get; set; } = "Arrangement 9 – Belt Drive";

    [Range(1000, 30000)]
    [Display(Name = "Max Tip Speed (fpm)")]
    public int MaxTipSpeed { get; set; } = 16000;

    [Range(0, 200)]
    [Display(Name = "Sound Limit (dBA)")]
    public int? SoundLimit { get; set; }

    [Range(1, 1000)]
    [Display(Name = "Quantity")]
    public int Qty { get; set; } = 1;

    public List<FanTypeViewModel> AvailableFanTypes { get; set; } = new();
}
