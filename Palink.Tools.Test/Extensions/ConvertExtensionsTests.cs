using System;
using System.ComponentModel;
using Palink.Tools.Extensions.ConvertExt;
using Xunit;

namespace Palink.Tools.Test.Extensions;

public class ConvertExtensionsTests
{
    [Fact]
    public void StringToValueType()
    {
        const string str = "123";
        var byteValue = str.ToByte();
        var intValue = str.ToInt();
        var longValue = str.ToLong();
        var doubleValue = str.ToDouble();
        var decimalValue = str.ToDecimal();
        var floatValue = str.ToFloat();
        var boolValue = str.ToBool(false, "123");

        Assert.Equal(typeof(byte), byteValue.GetType());
        Assert.Equal(typeof(int), intValue.GetType());
        Assert.Equal(typeof(long), longValue.GetType());
        Assert.Equal(typeof(decimal), decimalValue.GetType());
        Assert.Equal(typeof(double), doubleValue.GetType());
        Assert.Equal(typeof(float), floatValue.GetType());
        Assert.Equal(typeof(bool), boolValue.GetType());
    }

    [Fact]
    public void StringToDateTime()
    {
        const string time = "2022-04-16 13:49";
        var dateTime = time.ToDateTime(DateTime.Now);
    }

    [Fact]
    public void DateTimeToString()
    {
        var date = DateTime.Now;
        var time = date.ToDateString();
    }

    [Fact]
    public void TrimAndStringToEnum()
    {
        const string name = " N1 ";
        var n1 = name.Trim().TryToEnum<MyEnum>();

        var list = typeof(MyEnum).TryToList();
    }

    [Fact]
    public void ConvertibleTest()
    {
        const string num = "21221";
        const string b = "false";
        var d1 = num.To<double>();
        var b1 = b.To<bool>();
    }

    [Fact]
    public void ValueTupleString()
    {
        (int a1, int a2) value = (10, 20);
        var str = value.TupleTryToString();

        var tuple = str.TryToValueTuple<(int a1, int a2)>();
    }

    private enum MyEnum
    {
        [Description("N1")] N1,
        [Description("N2")] N2
    }
}