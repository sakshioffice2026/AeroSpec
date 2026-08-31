using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AeroSpec.Database;

public class PerformanceData
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("FanSize")]
    public int FanSizeId { get; set; }

    [Required]
    public int Rpm { get; set; }

    [Required]
    public double Volume { get; set; }

    [Required]
    public double StaticPressure { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public bool IsActive { get; set; } = true;

    public virtual FanSize? FanSize { get; set; }
}