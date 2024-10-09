using System.Numerics;

namespace DiscountCode.Domain.Generator;

public class UniqueCodeGenerator : ICodeGenerator
{
    private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    private const int TotalLength = 8;
    private BigInteger _currentNumber = BigInteger.Zero;

    public IEnumerable<string> GenerateCodes(int count)
    {
        var codes = new List<string>(count);
        
        for (var i = 0; i < count; i++)
        {
            codes.Add(GenerateCode());
        }
        
        return codes;
    }

    private string GenerateCode()
    {
        var code = ConvertToBase36(_currentNumber).PadLeft(TotalLength, '0');
        
        _currentNumber++;
        
        return code;
    }

    private static string ConvertToBase36(BigInteger number)
    {
        if (number == 0) return "0";

        var result = new List<char>();
        while (number > 0)
        {
            number = BigInteger.DivRem(number, 36, out var remainder);
            result.Insert(0, Chars[(int)remainder]);
        }

        return new string(result.ToArray());
    }
}