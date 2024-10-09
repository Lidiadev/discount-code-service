namespace DiscountCode.Application.Options;

public class DiscountCodeSettings
{
    public IList<int> UsedCodeLength { get; set; }
    public IList<int> CodeGenerationLengths { get; set; }
    public int MaxCodesPerRequest { get; set; }
    public int CodeCacheExpirationDays { get; set; }
    public int NotFoundCacheExpirationMinutes { get; set; }
    public int UsedCodeCacheExpirationDays { get; set; }
}