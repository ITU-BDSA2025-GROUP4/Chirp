namespace Utils.Tests;

using static StringUtils;

public class IsIntegerUnitTest
{
    [Theory]
    [InlineData("281")]
    [InlineData("9")]
    [InlineData("-1")]
    [InlineData("0")]
    public void GeneralUseTrue(string x)
    {
        Assert.True(IsInteger(x));
    }

    [Theory]
    [InlineData("--281")]
    [InlineData("abc")]
    [InlineData("4.5")]
    [InlineData("3-")]
    public void GeneralUseFalse(string x)
    {
        Assert.False(IsInteger(x));
    }

    [Theory]
    [InlineData(" 2")]
    [InlineData("1 ")]
    [InlineData("- 5")]
    public void EdgeCasesFalse(string x)
    {
        Assert.False(IsInteger(x));
    }

    [Theory]
    [InlineData("-0")]
    public void EdgeCasesTrue(string x)
    {
        Assert.True(IsInteger(x));
    }
}