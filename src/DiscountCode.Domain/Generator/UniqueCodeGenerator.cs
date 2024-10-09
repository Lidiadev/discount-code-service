using System.Numerics;

namespace DiscountCode.Domain.Generator;

public class UniqueCodeGenerator : ICodeGenerator
{
    private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    private BigInteger _currentNumber = BigInteger.Zero;

    public IEnumerable<string> GenerateCodes(int count, int length)
    {
        var codes = new List<string>(count);
        
        for (var i = 0; i < count; i++)
        {
            codes.Add(GenerateCode(length));
        }
        
        return codes;
    }

    private string GenerateCode(int length)
    {
        var code = ConvertToBase36(_currentNumber).PadLeft(length, '0');
        
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