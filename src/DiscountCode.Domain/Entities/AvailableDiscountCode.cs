namespace DiscountCode.Domain.Entities;

public class AvailableDiscountCode
{
    public long Id { get; private set; }
    public string Code { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private AvailableDiscountCode() { } 

    public static AvailableDiscountCode Create(string code, DateTime createdAt)
    {
        return new AvailableDiscountCode
        {
            Code = code,
            CreatedAt = createdAt
        };
    }
}