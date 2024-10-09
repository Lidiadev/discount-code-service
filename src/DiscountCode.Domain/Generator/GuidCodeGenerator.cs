namespace DiscountCode.Domain.Generator;

public class GuidCodeGenerator : ICodeGenerator
{
    private readonly IDateTimeProvider _dateTimeProvider;
    
    public GuidCodeGenerator(IDateTimeProvider dateTimeProvider)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    public IEnumerable<string> GenerateCodes(int count, int length)
    {
        var codes = new HashSet<string>();
        while (codes.Count < count)
        {
            codes.Add(GenerateCode(length));
        }
        
        return codes;
    }

    private string GenerateCode(int length)
    {
        var timestamp = _dateTimeProvider.Ticks;
        var guid = Guid.NewGuid().ToString("N");
        var code = $"{timestamp:X}{guid.Substring(0, 6)}".ToUpper();
        
        return code.Substring(0, length);
    }
}