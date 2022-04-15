using Microsoft.Win32;
using Palink.Tools.Utils;
using Xunit;

namespace Palink.Tools.Test.Utils;

public partial class CoreToolTest
{
    [Fact]
    public void CreateFolderInLocalMachineTest()
    {
        CoreTool.CreateFolderInLocalMachine(
            "SOFTWARE\\Policies\\Microsoft\\Windows\\PalinkTest");
    }

    [Fact]
    public void DeleteFolderInLocalMachineTest()
    {
        CoreTool.DeleteFolderInLocalMachine(
            "SOFTWARE\\Policies\\Microsoft\\Windows\\PalinkTest");
    }

    [Fact]
    public void SetupKeyValueInLocalMachineTest()
    {
        CoreTool.SetupKeyValueInLocalMachine(
            "SOFTWARE\\Policies\\Microsoft\\Windows\\PalinkTest", "Palinks", 0,
            RegistryValueKind.DWord);
    }
}