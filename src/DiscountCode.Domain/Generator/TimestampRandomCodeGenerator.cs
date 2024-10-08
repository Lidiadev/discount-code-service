namespace DiscountCode.Domain.Generator;

public class TimestampRandomCodeGenerator : ICodeGenerator
{
    private readonly IDateTimeProvider _dateTimeProvider;
    private static readonly Random Random = new();
    private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    public TimestampRandomCodeGenerator(IDateTimeProvider dateTimeProvider)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    public IEnumerable<string> GenerateCodes(int count)
    {
        var codes = new HashSet<string>();
        while (codes.Count < count)
        {
            codes.Add(GenerateCode());
        }
        
        return codes;
    }

    private string GenerateCode()
    {
        var timestamp = _dateTimeProvider.Ticks;
        var timestampPart = timestamp
            .ToString("X")
            .PadRight(12, '0')
            .Substring(0, 4);
        
        var randomPart = new string(Enumerable.Repeat(Chars, 4)
            .Select(s => s[Random.Next(s.Length)])
            .ToArray());
        
        return timestampPart + randomPart;
    }
}