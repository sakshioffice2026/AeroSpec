using System.ComponentModel.DataAnnotations;

namespace AeroSpec.Database;

public class FanSize
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string SizeId { get; set; } = string.Empty;

    [Required]
    public double DiameterIn { get; set; }

    [Required]
    public double CfmScale { get; set; }

    [Required]
    public double SpScale { get; set; }

    [Required]
    public int MaxRpm { get; set; }

    [Required]
    public double OutletArea { get; set; }

    [Required]
    public int WeightBase { get; set; }

    [Required]
    public int BasePrice { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public DateTime ModifiedDate { get; set; } = DateTime.UtcNow;

    public bool IsActive { get; set; } = true;

    public virtual ICollection<PerformanceData> PerformanceDataSet { get; set; } = new List<PerformanceData>();
}