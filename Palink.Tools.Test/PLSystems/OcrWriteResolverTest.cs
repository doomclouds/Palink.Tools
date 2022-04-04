using Newtonsoft.Json;
using Palink.Tools.PLSystems.Attribute;
using Xunit;

namespace Palink.Tools.Test.PLSystems;

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

        var jt1 = JsonConvert.DeserializeObject<JsonTest>(j1, jssRead);
        var jt2 = JsonConvert.DeserializeObject<JsonTest>(j2, jssWrite);
    }
}

public class JsonTest
{
    [JsonOcr(ReadName = "rName", WriteName = "wName")]
    public string Name { get; set; } = "Bob";

    [JsonOcr(ReadName = "rAge", WriteName = "wAge")]
    public string Age { get; set; } = "10";
}