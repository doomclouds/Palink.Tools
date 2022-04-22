using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using Palink.Tools.Extensions.PLObject;
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

    [Fact]
    public void DeepCloneTest()
    {
        var objTest = new ObjectTest();
        var ot1 = objTest.DeepClone();
        var ot2 = objTest.DeepClone<ObjectTest>();

        Assert.False(objTest.ReferenceEquals(ot1));
        Assert.False(objTest.ReferenceEquals(ot2));
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