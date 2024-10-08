using System.ComponentModel.DataAnnotations;

namespace DiscountCode.Infrastructure.Persistance.Models;

public class AvailableDiscountCodeModel
{
    public long Id { get; set; }
    
    [Required]
    [StringLength(50)]
    public string Code { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; }
}