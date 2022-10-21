using System.Collections.Generic;
using System.Linq;

namespace Palink.Tools.Utility;

/// <summary>
/// CoreTool
/// </summary>
public partial class CoreTool
{
    public static string BuilderFileFilter(List<string> filters, string category,
        bool addAll = false)
    {
        var filter = $"{category}|";
        filter = filters.Aggregate(filter, (current, f) => current + $"*.{f};");
        if (addAll)
        {
            filter += "|所有|*.*";
        }

        return filter;
    }

    public static string BuilderFileFilter(
        List<(List<string> filters, string category)> categories, bool addAll = false)
    {
        var filter = "";
        var index = 0;
        foreach (var valueTuple in categories)
        {
            index++;
            filter += $"{valueTuple.category}|";
            filter = valueTuple.filters.Aggregate(filter,
                (current, f) => current + $"*.{f};");

            if (index != categories.Count)
                filter += "|";
        }

        if (addAll)
        {
            filter += "|所有|*.*";
        }

        return filter;
    }
}