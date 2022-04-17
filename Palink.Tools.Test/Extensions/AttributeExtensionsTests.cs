using System.ComponentModel;
using Palink.Tools.Extensions.PLAttribute;
using Xunit;

namespace Palink.Tools.Test.Extensions;

public class AttributeExtensionsTests
{
    [Fact]
    public void EnumTests()
    {
        var testModeName1 = TestMode.Normal.EnumDescription();
        var testModeName2 = TestMode.Plus.EnumDescription();
    }
}

public enum TestMode
{
    [Description("普通测试")]
    Normal,
    [Description("加强版测试")]
    Plus
}