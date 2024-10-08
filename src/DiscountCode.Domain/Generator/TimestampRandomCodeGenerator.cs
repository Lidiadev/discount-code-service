using System.Security.Cryptography;

namespace DiscountCode.Domain.Generator;

public class TimestampRandomCodeGenerator : ICodeGenerator
{
    private readonly IDateTimeProvider _dateTimeProvider;
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
        
        var randomPart = GenerateRandomPart(6); 
        
        return timestampPart + randomPart;
    }

    private string GenerateRandomPart(int length)
    {
        var randomBytes = new byte[length];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }

        return new string(randomBytes.Select(b => Chars[b % Chars.Length]).ToArray());
    }
}