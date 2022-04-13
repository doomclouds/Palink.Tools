using System;
using Palink.Tools.System.PLTime;
using Xunit;

namespace Palink.Tools.Test.System;

public class ChineseCalendarTests
{
    [Fact]
    public void ChineseCalendarTest()
    {
        var calendar = new ChineseCalendar(new DateTime(1993, 5, 9));
        var animal = calendar.AnimalString;
        var date = calendar.ChineseDateString;
        var constellation = calendar.ChineseConstellation;
        var tg = calendar.GanZhiYearString;
        Assert.Equal(animal, "鸡");
        Assert.Equal(date, "一九九三年闰三月十八");
        Assert.Equal(constellation, "柳土獐");
        Assert.Equal(tg, "癸酉年");
    }
}