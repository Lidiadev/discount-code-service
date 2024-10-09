namespace DiscountCode.BackgroundServices.Options;

public class DiscountCodeGenerationSettings
{
    public int CodeGenerationIntervalSeconds { get; set; }
    public int CodeGenerationBatchSize { get; set; }
    public List<int> CodeGenerationLengths { get; set; }
}