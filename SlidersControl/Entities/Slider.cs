using SlidersControl.Entities.Enum;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SlidersControl.Entities;

public class Slider
{
    [Key]
    public Guid Id { get; set; }

    [Column("large_image")]
    public string LargeImage { get; set; }

    [Column("smal_image")]
    public string SmalImage { get; set; }

    public Order? Order { get; set; }

    [Column("access_link")]
    public string? AccessLink { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = false;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
