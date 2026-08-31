using System.ComponentModel.DataAnnotations;


namespace AeroSpec.Database;

public class FanSelection
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(255)]
    public string ProjectName { get; set; } = string.Empty;

    [StringLength(100)]
    public string? Tag { get; set; }

    [Required]
    public double RequiredCfm { get; set; }

    [Required]
    public double RequiredSp { get; set; }

    public int Elevation { get; set; }

    public int Temperature { get; set; }

    [Required]
    [StringLength(50)]
    public string FanType { get; set; } = "housed_belt";

    [Required]
    [StringLength(100)]
    public string Arrangement { get; set; } = "Arrangement 9 – Belt Drive";

    public int MaxTipSpeed { get; set; } = 16000;

    public int? SoundLimit { get; set; }

    public int Quantity { get; set; } = 1;

    [Required]
    [StringLength(50)]
    public string SelectedFanId { get; set; } = string.Empty;

    public int SelectedRpm { get; set; }

    public double SelectedBhp { get; set; }

    public int SelectedMotorHp { get; set; }

    public double SelectedEfficiency { get; set; }

    public int SelectedTipSpeed { get; set; }

    public int SelectedSound { get; set; }

    public double DensityRatio { get; set; } = 1.0;

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public DateTime ModifiedDate { get; set; } = DateTime.UtcNow;

    public bool IsActive { get; set; } = true;
}