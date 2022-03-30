using System.ComponentModel;
using Palink.Tools.Extensions;
using Xunit;

namespace Palink.Tools.Test.Extensions;

public class ObjectExtensionsTests
{
    [Fact]
    public void PropDescriptionDisplayNameTest()
    {
        var objTest = new ObjectTest();
        var title1 = objTest.PropertyDescription(nameof(objTest.Title));
        var title2 = objTest.PropertyDescription(nameof(objTest.Title));

        Assert.Equal(title1, title2);
        Assert.Equal(title1, "标题");
    }
}

public enum TestMethod
{
    [Description("无")]
    None,
    [Description("测试")]
    Test
}

internal class ObjectTest
{
    [Description("标题")]
    [DisplayName("标题")]
    public string Title { get; set; }
}