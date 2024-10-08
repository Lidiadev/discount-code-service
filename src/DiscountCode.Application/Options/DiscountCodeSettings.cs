namespace DiscountCode.Application.Options;

public class DiscountCodeSettings
{
    public int CodeLength { get; set; }
    public int MaxCodesPerRequest { get; set; }
    public int CodeCacheExpirationDays { get; set; }
    public int NotFoundCacheExpirationMinutes { get; set; }
    public int UsedCodeCacheExpirationDays { get; set; }
}