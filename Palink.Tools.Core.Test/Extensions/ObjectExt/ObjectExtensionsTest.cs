using System.ComponentModel;
using Palink.Tools.Extensions.ObjectExt;

namespace Palink.Tools.Core.Test.Extensions.ObjectExt;

public class ObjectExtensionsTest
{
    [Fact]
    public void PropDescriptionDisplayNameTest()
    {
        var user = new User();
        var desc = user.PropertyDescription(nameof(user.Name));
        var dsp = user.PropertyDisplayName(nameof(user.Name));
        Assert.Equal(desc, "用户名");
        Assert.Equal(dsp, "昵称");
    }
}

public class User
{
    [Description("用户名")]
    [DisplayName("昵称")]
    public string? Name { get; set; } = "";
}