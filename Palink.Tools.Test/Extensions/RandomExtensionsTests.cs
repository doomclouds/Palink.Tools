using System;
using Palink.Tools.Extensions.PLRandom;
using Xunit;

namespace Palink.Tools.Test.Extensions;

public class RandomExtensionsTests
{
    [Fact]
    public void StrictRandomTest()
    {
        var _ = new Random(10000);
        var r1 = _.StrictNext(1, 37);
        var r2 = _.StrictNext(1, 37);
        var r3 = _.StrictNext(1, 37);
        var r4 = _.StrictNext(1, 37);
        var r5 = _.StrictNext(1, 37);
    }
}