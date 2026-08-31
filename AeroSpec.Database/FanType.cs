using System.ComponentModel.DataAnnotations;

namespace AeroSpec.Database;

public class FanType
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string TypeId { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Label { get; set; } = string.Empty;

    [Required]
    [StringLength(10)]
    public string Prefix { get; set; } = string.Empty;

    [Required]
    public double SpMod { get; set; }

    [Required]
    public double EffMod { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public DateTime ModifiedDate { get; set; } = DateTime.UtcNow;

    public bool IsActive { get; set; } = true;
}