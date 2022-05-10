using Palink.Tools.Extensions.PLSecurity;
using Xunit;

namespace Palink.Tools.Test.Extensions;

public class EncryptExtensionsTests
{
    [Fact]
    public void EncryptTest()
    {
        const string content = "test content";
        const string pwd = "palink@&";
        const string iv = "5BFFC92D65DC";

        var encrypt = content.AESEncrypt(pwd, iv);
        var decrypt = encrypt.AESDecrypt(pwd, iv);

        Assert.Equal(content, decrypt);
    }
}