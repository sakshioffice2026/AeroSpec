using System.ComponentModel.DataAnnotations;

namespace AeroSpec.Web.ViewModels;

public class FanTypeViewModel
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    [Display(Name = "Type ID")]
    public string TypeId { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Label { get; set; } = string.Empty;

    [Required]
    [StringLength(10)]
    public string Prefix { get; set; } = string.Empty;

    [Required]
    [Display(Name = "SP Modifier")]
    public double SpMod { get; set; }

    [Required]
    [Display(Name = "Efficiency Modifier")]
    public double EffMod { get; set; }
}
