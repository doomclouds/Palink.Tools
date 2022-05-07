using System.IO;
using Microsoft.Extensions.Configuration;

namespace Palink.Tools.System.PLConfiguration;

/// <summary>
/// appsettings.json configuration file data reading class
/// </summary>
public abstract class JsonAppSettings
{
    /// <summary>
    /// The root node of the configuration file
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