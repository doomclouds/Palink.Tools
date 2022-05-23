using Newtonsoft.Json;
using Palink.Tools.System.AttributeExt;
using Xunit;

namespace Palink.Tools.Test.System;

public class OcrWriteResolverTest
{
    [Fact]
    public void ReadJsonTest()
    {
        var jsonTest = new JsonTest();
        var jssRead = new JsonSerializerSettings()
        {
            ContractResolver = new JsonOcrReadResolver()
        };
        var jssWrite = new JsonSerializerSettings()
        {
            ContractResolver = new JsonOcrWriteResolver()
        };
        var j1 = JsonConvert.SerializeObject(jsonTest, jssRead);
        var j2 = JsonConvert.SerializeObject(jsonTest, jssWrite);

        Assert.False(j1 == j2);
        var jt1 = JsonConvert.DeserializeObject<JsonTest>(j1, jssRead);
        var jt2 = JsonConvert.DeserializeObject<JsonTest>(j2, jssWrite);
        Assert.True(jt1.Age == jt2.Age);
        Assert.True(jt1.Name == jt2.Name);
    }
}

public class JsonTest
{
    [JsonOcr(ReadName = "rName", WriteName = "wName", Readable = true, Writable = false)]
    public string Name { get; set; } = "Bob";

    [JsonOcr(ReadName = "rAge", WriteName = "wAge", Readable = true, Writable = false)]
    public string Age { get; set; } = "10";
}