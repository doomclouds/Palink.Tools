using System;
using Palink.Tools.Extensions.PLConvert;
using Xunit;

namespace Palink.Tools.Test.Extensions;

public class ConvertExtensionsTest
{
    [Fact]
    public void NormalTest()
    {
        var time = "90";
        var t = time.TryConvertTo<double>();

        Assert.True(Math.Abs(t - 90) == 0);
    }
}