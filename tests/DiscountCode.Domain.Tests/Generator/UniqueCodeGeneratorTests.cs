using DiscountCode.Domain.Generator;

namespace DiscountCode.Domain.Tests.Generator;

public class UniqueCodeGeneratorTests
{
    private const int CodeCount = 1000000; 
    private const int CodeLength = 8;
    private readonly ICodeGenerator _generator;

    public UniqueCodeGeneratorTests()
    {
        _generator = new UniqueCodeGenerator();
    }

    [Fact]
    public void GenerateCodes_Should_Have_No_Collisions()
    {
        var codes = _generator.GenerateCodes(CodeCount, CodeLength).ToList();

        Assert.Equal(CodeCount, codes.Distinct().Count());
    }

    [Fact]
    public void GenerateCodes_Should_Generate_Valid_Length_Codes()
    {
        var codes = _generator.GenerateCodes(100, CodeLength).ToList();
        
        Assert.All(codes, code => Assert.Equal(CodeLength, code.Length));
    }
}