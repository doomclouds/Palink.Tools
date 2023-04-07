using System.Text.RegularExpressions;
using Palink.Tools.Extensions.StringExt;
using Xunit.Abstractions;

namespace Palink.Tools.Core.Test.Extensions.StringExt;

public class StringExtensionsTest
{
    private readonly ITestOutputHelper _testOutputHelper;

    public StringExtensionsTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void BuildRegexBetweenLeftToRightTest()
    {
        const string msg = "open:[01]:[17]:[11223344556677ff]";
        var regex = StringExtensions.BuildRegexBetweenLeftToRight();
        var matches = Regex.Matches(msg, regex);
        Assert.Equal(matches[0].ToString(), "01");
        Assert.Equal(matches[1].ToString(), "17");
        Assert.Equal(matches[2].ToString(), "11223344556677ff");
    }
}