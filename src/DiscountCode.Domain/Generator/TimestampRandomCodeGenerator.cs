namespace DiscountCode.Domain.Generator;

public class TimestampRandomCodeGenerator : ICodeGenerator
{
    private static readonly Random Random = new();
    private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    public IEnumerable<string> GenerateCodes(int count)
    {
        var codes = new HashSet<string>();
        while (codes.Count < count)
        {
            codes.Add(GenerateCode());
        }
        
        return codes;
    }

    // TODO: NOT UNIQUE - getting insert errors!!
    private static string GenerateCode()
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var timestampPart = timestamp.ToString("X").PadLeft(8, '0').Substring(0, 4);
        var randomPart = new string(Enumerable.Repeat(Chars, 4)
            .Select(s => s[Random.Next(s.Length)]).ToArray());
        return timestampPart + randomPart;
    }
    
    // public string GenerateCode()
    // {
    //     var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString("X");
    //     var random = new Random();
    //     const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    //     var randomPart = new string(Enumerable.Repeat(chars, 4)
    //         .Select(s => s[random.Next(s.Length)])
    //         .ToArray());
    //     
    //     return $"{timestamp.Substring(timestamp.Length - 4)}{randomPart}";
    // }
}