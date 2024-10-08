using System.ComponentModel.DataAnnotations;

namespace DiscountCode.Infrastructure.Persistance.Models;

public class DiscountCodeModel
{
    public long Id { get; set; }
    
    [Required]
    [StringLength(50)]
    public string Code { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; }
    
    public bool IsUsed { get; set; }
    
    public DateTime? UsedAt { get; set; }
    
    [Timestamp]
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}