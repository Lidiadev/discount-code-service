namespace DiscountCode.Domain.Generator;

public interface ICodeGenerator
{
    IEnumerable<string> GenerateCodes(int count, int length);
}