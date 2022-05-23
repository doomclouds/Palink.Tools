using Palink.Tools.Extensions.SerializeExt;
using Xunit;

namespace Palink.Tools.Test.Extensions;

public class SerializeExtensionsTests
{
    [Fact]
    public void JsonToTupleTest()
    {
        var json = "{\"Rgb\":{\"r\":100,\"g\":200,\"b\":50}}";
        var obj = json.FromJson<MyClass>();
    }

    [Fact]
    public void TupleToJsonTest()
    {
        var myClass = new MyClass()
        {
            Rgb = (1, 1, 1),
        };
        var json = myClass.ToJson(true);
        var obj = json.FromJson<MyClass>();
    }

    [Fact]
    public void ByteArrayStringTest()
    {
        var msg = "Palink";
        var bytes = msg.SerializeUtf8();
        var msg2 = bytes.DeserializeUtf8();
        Assert.Equal(msg, msg2);
    }

    public class MyClass
    {
        public (byte r, byte g, byte b) Rgb { get; set; }
    }
}