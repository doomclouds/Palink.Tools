using System.IO;
using Microsoft.Extensions.Configuration;

namespace Palink.Tools.System.PLConfiguration;

/// <summary>
/// appsettings.json配置文件数据读取类
/// </summary>
public abstract class JsonAppSettings
{
    /// <summary>
    /// 配置文件的根节点
    /// </summary>
    public static readonly IConfigurationRoot Config;

    /// <summary>
    /// Constructor
    /// </summary>
    static JsonAppSettings()
    {
        // 加载appsettings.json，并构建IConfigurationRoot
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", true, true);
        Config = builder.Build();
    }
}