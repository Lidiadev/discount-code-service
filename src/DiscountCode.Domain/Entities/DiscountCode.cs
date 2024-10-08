namespace DiscountCode.Domain.Entities;

public class DiscountCode
{
    public long Id { get; private set; }
    public string Code { get; private set; }
    public bool IsUsed => UsedAt.HasValue;
    public DateTime CreatedAt { get; private set; }
    public DateTime? UsedAt { get; private set; }
    
    private DiscountCode() { }
    
    public static DiscountCode Create(string code, DateTime createdAt)
    {
        return new DiscountCode
        {
            Code = code,
            CreatedAt = createdAt,
        };
    }

    public void MarkAsUsed(DateTime usedAt)
    {
        if (IsUsed)
        {
            throw new InvalidOperationException("Code already used");
        }

        UsedAt = usedAt;
    }
}