using DiscountCode.Domain;

namespace DiscountCode.Infrastructure;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;

    public long Ticks => DateTimeOffset.UtcNow.Ticks;
}