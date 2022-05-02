using System;
using System.ComponentModel;
using Palink.Tools.Extensions.PLConvert;
using Xunit;

namespace Palink.Tools.Test.Extensions;

public class ConvertExtensionsTests
{
    [Fact]
    public void StringToValueType()
    {
        const string str = "123";
        var byteValue = str.TryToByte();
        var intValue = str.TryToInt();
        var longValue = str.TryToLong();
        var doubleValue = str.TryToDouble();
        var decimalValue = str.TryToDecimal();
        var floatValue = str.TryToFloat();
        var boolValue = str.TryToBool(false, "123");

        Assert.Equal(typeof(byte), byteValue.GetType());
        Assert.Equal(typeof(int), intValue.GetType());
        Assert.Equal(typeof(long), longValue.GetType());
        Assert.Equal(typeof(decimal), decimalValue.GetType());
        Assert.Equal(typeof(double), doubleValue.GetType());
        Assert.Equal(typeof(float), floatValue.GetType());
        Assert.Equal(typeof(bool), boolValue.GetType());
    }

    [Fact]
    public void ValueTypeToString()
    {
        const byte b = 100;
        var bStr = b.TryToString();
        const bool mB = false;
        var mBStr = mB.TryToString();

        Assert.Equal(typeof(string), bStr.GetType());
        Assert.Equal(typeof(string), mBStr.GetType());
    }

    [Fact]
    public void StringToDateTime()
    {
        const string time = "2022-04-16 13:49";
        var dateTime = time.TryToDateTime(DateTime.Now);
    }

    [Fact]
    public void DateTimeToString()
    {
        var date = DateTime.Now;
        var time = date.TryToDateTime("yyyy-MM-dd HH:mm");
    }

    [Fact]
    public void TrimAndStringToEnum()
    {
        const string name = " N1 ";
        var n1 = name.TryToTrim().TryToEnum<MyEnum>();

        var list = typeof(MyEnum).TryToList();
    }

    [Fact]
    public void ConvertibleTest()
    {
        const string num = "21221";
        const string b = "false";
        const string c = "1";
        var d1 = num.ConvertTo<double>();
        var b1 = b.ConvertTo<bool>();
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