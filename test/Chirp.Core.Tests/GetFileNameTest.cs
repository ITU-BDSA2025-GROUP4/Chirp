namespace Utils.Tests;

using Chirp.Core.Utils;

public class GetFileNameUnitTest
{
    [Theory]
    [InlineData("dir/file.txt", "file.txt")]
    [InlineData("/inroot/dir/foo.png", "foo.png")]
    [InlineData("bar.txt", "bar.txt")]
    [InlineData("/who / put / these / spaces / in here.txt", " in here.txt")]
    [InlineData("windows\\is\\all\\backwards.jpeg", "backwards.jpeg")]
    [InlineData("\\example.txt", "example.txt")]
    public void GeneralUseCase(string input, string expectedOutput)
    {
        Assert.Equal(StringUtils.GetFileName(input), expectedOutput);
    }

    [Theory]
    [InlineData("", "")]
    [InlineData("/", "")]
    [InlineData("///", "")]
    [InlineData("\\", "")]
    [InlineData("\\\\", "")]
    [InlineData("foo/.txt", ".txt")]
    public void EdgeUseCase(string input, string expectedOutput)
    {
        Assert.Equal(StringUtils.GetFileName(input), expectedOutput);
    }
}