using Palink.Tools.Extensions.PLArray;
using Palink.Tools.Extensions.PLConvert;
using Xunit;

namespace Palink.Tools.Test.Extensions;

public class ArrayExtensionsTests
{
    [Fact]
    public void ForEachTest()
    {
        var array = new[]
        {
            1,
            2,
            3,
            4,
            5
        };

        var array2 = new[,]
        {
            {1,2,3},{4,5,6},{7,8,9}
        };

        array.ForEach((arr, indices) =>
        {
            var value = arr.GetValue(indices[0]);
            arr.SetValue(value.ToInt() + 2, indices[0]);
        });

        //对二维按列遍历
        array2.ForEach((arr, indices) =>
        {
            var value = arr.GetValue(indices[0], indices[1]);
            arr.SetValue(value.ToInt() + 2, indices[0], indices[1]);
        });
    }
}