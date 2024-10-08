namespace DiscountCode.Domain;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}