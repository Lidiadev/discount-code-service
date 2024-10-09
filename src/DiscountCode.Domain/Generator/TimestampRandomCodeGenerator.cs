using System.Security.Cryptography;

namespace DiscountCode.Domain.Generator;

public class TimestampRandomCodeGenerator : ICodeGenerator
{
    private readonly IDateTimeProvider _dateTimeProvider;
    private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    private const int TimestampPartLength = 4; 
    private const int PaddingLength = 12; 
    
    public TimestampRandomCodeGenerator(IDateTimeProvider dateTimeProvider)
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
        var timestampPart = timestamp
            .ToString("X")
            .PadRight(PaddingLength, '0')
            .Substring(0, TimestampPartLength);
        
        var randomPart = GenerateRandomPart(length - TimestampPartLength); 
        
        return timestampPart + randomPart;
    }

    private static string GenerateRandomPart(int length)
    {
        var randomBytes = new byte[length];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }

        return new string(randomBytes.Select(b => Chars[b % Chars.Length]).ToArray());
    }
}