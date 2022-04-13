using System.Linq;
using Palink.Tools.System.Mime;
using Xunit;

namespace Palink.Tools.Test.System;

public class MimeMapperTests
{
    [Fact]
    public void MimeMapperTest()
    {
        var t = DefaultMimeItems.Items.Where(x => x.MimeType == ContentType.Bmp);
        var mime = new MimeMapper(DefaultMimeItems.Items.ToArray());
        var mimeType = mime.GetMimeFromPath("c:/1.bmp");
        Assert.Equal(mimeType, ContentType.Bmp);

        mimeType = mime.GetMimeFromExtension("bmp");
        Assert.Equal(mimeType, ContentType.Bmp);

        var extension = mime.GetExtensionFromMime(mimeType);
        Assert.True(extension.Contains("bmp"));
        Assert.True(extension.Contains("dib"));
    }
}