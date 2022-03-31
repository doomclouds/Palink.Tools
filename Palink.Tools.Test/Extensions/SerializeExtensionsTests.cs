using Palink.Tools.Extensions;
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

    public class MyClass
    {
        public (byte r, byte g, byte b) Rgb { get; set; }
    }
}