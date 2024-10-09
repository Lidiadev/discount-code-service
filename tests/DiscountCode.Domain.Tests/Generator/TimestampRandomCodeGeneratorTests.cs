using DiscountCode.Domain.Generator;
using Moq;

namespace DiscountCode.Domain.Tests.Generator;

public class TimestampRandomCodeGeneratorTests
{
    private const int CodeCount = 1000000; 
    private const int CodeLength = 8;
    private const long Ticks = 638640971304022638;
    private readonly ICodeGenerator _generator;
    private readonly Mock<IDateTimeProvider> _dateTimeProviderMock;

    public TimestampRandomCodeGeneratorTests()
    {
        _dateTimeProviderMock = new Mock<IDateTimeProvider>();
        _generator = new TimestampRandomCodeGenerator(_dateTimeProviderMock.Object);
    }

    [Fact]
    public void GenerateCodes_Should_Have_No_Collisions()
    {
        _dateTimeProviderMock.Setup(m => m.Ticks).Returns(Ticks);
        var codes = _generator.GenerateCodes(CodeCount, CodeLength).ToList();

        Assert.Equal(CodeCount, codes.Distinct().Count());
    }

    [Fact]
    public void GenerateCodes_Should_Generate_Valid_Length_Codes()
    {
        _dateTimeProviderMock.Setup(m => m.Ticks).Returns(Ticks);
        var codes = _generator.GenerateCodes(100, CodeLength).ToList();
        
        Assert.All(codes, code => Assert.Equal(CodeLength, code.Length));
    }
}