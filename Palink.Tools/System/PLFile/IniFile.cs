using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Palink.Tools.System.PLFile;

/// <summary>
/// INIFile
/// </summary>
public class IniFile
{
    /// <summary>
    /// 传入INI文件路径构造对象,路径示例c:/test/test.ini
    /// </summary>
    /// <param name="iniPath">INI文件路径</param>
    public IniFile(string iniPath)
    {
        Path = iniPath;

        if (!File.Exists(Path))
        {
            File.Create(Path);
        }
    }

    /// <summary>
    /// 获取路径
    /// </summary>
    public string Path { get; }

    [DllImport("kernel32")]
    private static extern long WritePrivateProfileString(string? section, string? key,
        string? val, string filePath);

    [DllImport("kernel32")]
    private static extern int GetPrivateProfileString(string section, string key,
        string def, StringBuilder retVal, int size, string filePath);

    [DllImport("kernel32")]
    private static extern int GetPrivateProfileString(string section, string key,
        string defVal, byte[] retVal, int size, string filePath);

    /// <summary>
    /// 写INI文件
    /// </summary>
    /// <param name="section">分组节点</param>
    /// <param name="key">关键字</param>
    /// <param name="value">值</param>
    public void IniWriteValue(string? section, string? key, string? value)
    {
        WritePrivateProfileString(section, key, value, Path);
    }

    /// <summary>
    /// 读取INI文件
    /// </summary>
    /// <param name="section">分组节点</param>
    /// <param name="key">关键字</param>
    /// <returns>值</returns>
    public string IniReadValue(string section, string key)
    {
        var temp = new StringBuilder(255);
        GetPrivateProfileString(section, key, "", temp, 255, Path);
        return temp.ToString();
    }

    /// <summary>
    /// 读取INI文件
    /// </summary>
    /// <param name="section">分组节点</param>
    /// <param name="key">关键字</param>
    /// <returns>值的字节表现形式</returns>
    public byte[] IniReadValues(string section, string key)
    {
        var temp = new byte[255];
        GetPrivateProfileString(section, key, "", temp, 255, Path);
        return temp;
    }

    /// <summary>
    /// 删除ini文件下所有段落
    /// </summary>
    public void ClearAllSection()
    {
        IniWriteValue(null, null, null);
    }

    /// <summary>
    /// 删除ini文件下指定段落下的所有键
    /// </summary>
    /// <param name="section">分组节点</param>
    public void ClearSection(string? section)
    {
        IniWriteValue(section, null, null);
    }
}