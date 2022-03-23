using Microsoft.Win32;

namespace Palink.Tools.Utils;

/// <summary>
/// CoreTool
/// </summary>
public partial class CoreTool
{
    /// <summary>
    /// CreateFolderInLocalMachine
    /// </summary>
    /// <param name="targetPath"></param>
    public static void CreateFolderInLocalMachine(string targetPath)
    {
        var localMachine = Registry.LocalMachine;

        if (localMachine.OpenSubKey(targetPath) == null)
        {
            localMachine.CreateSubKey(targetPath, true);
            localMachine.Close();
        }
    }

    /// <summary>
    /// DeleteFolderInLocalMachine
    /// </summary>
    /// <param name="targetPath"></param>
    public static void DeleteFolderInLocalMachine(string targetPath)
    {
        var localMachine = Registry.LocalMachine;

        if (localMachine.OpenSubKey(targetPath) != null)
        {
            localMachine.DeleteSubKey(targetPath, true);
            localMachine.Close();
        }
    }

    /// <summary>
    /// SetupKeyValueInLocalMachine
    /// </summary>
    /// <param name="targetPath"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="kind"></param>
    /// <returns></returns>
    public static bool SetupKeyValueInLocalMachine(string targetPath, string key,
        object value, RegistryValueKind kind)
    {
        var localMachine = Registry.LocalMachine;
        var reg = localMachine.OpenSubKey(targetPath, true);
        if (reg != null)
        {
            reg.SetValue(key, value, kind);
            return true;
        }

        return false;
    }
}