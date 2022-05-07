using System;
using System.Linq;
using Microsoft.Win32;
using Microsoft.Win32.TaskScheduler;

namespace Palink.Tools.Utility;

/// <summary>
/// CoreTool
/// </summary>
public class TaskSchedulerTool
{
    #region 任务计划

    /// <summary>
    /// 添加计划任务
    /// </summary>
    /// <param name="taskName"></param>
    /// <param name="exeFullName"></param>
    /// <param name="description"></param>
    /// <param name="delay"></param>
    /// <returns></returns>
    internal static bool AddTask(string taskName, string exeFullName, string description,
        TimeSpan delay)
    {
        try
        {
            if (UpdateTask(taskName, description, delay))
            {
                return true;
            }

            var taskService = new TaskService();
            var taskDefinition = taskService.NewTask();
            taskDefinition.Triggers.Add(new BootTrigger
            {
                Delay = delay
            });
            taskDefinition.RegistrationInfo.Description = description;
            taskDefinition.Settings.DisallowStartIfOnBatteries = false;
            var logonTrigger = new LogonTrigger();
            taskDefinition.Triggers.Add(logonTrigger);
            taskDefinition.Settings.StopIfGoingOnBatteries = false;
            taskDefinition.Principal.RunLevel = TaskRunLevel.Highest;
            taskDefinition.Principal.GroupId = "Administrators";
            taskDefinition.Actions.Add(new ExecAction(exeFullName));
            taskService.RootFolder.RegisterTaskDefinition(taskName, taskDefinition);
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception.Message);
            return false;
        }

        return true;
    }

    /// <summary>
    /// 修改已存在计划任务
    /// </summary>
    /// <param name="taskName"></param>
    /// <param name="description"></param>
    /// <param name="delay"></param>
    /// <returns></returns>
    internal static bool UpdateTask(string taskName, string description, TimeSpan delay)
    {
        var taskService = TaskService.Instance;
        var task =
            taskService.RootFolder.Tasks.FirstOrDefault(task =>
                task.Name.Equals(taskName));

        if (task != null)
        {
            task.Definition.RegistrationInfo.Description = description;
            task.Definition.Triggers.Clear();
            task.Definition.Triggers.Add(new BootTrigger
            {
                Delay = delay
            });
            taskService.RootFolder.RegisterTaskDefinition(taskName, task.Definition);
            return true;
        }

        return false;
    }

    /// <summary>
    /// 删除计划任务
    /// </summary>
    /// <param name="taskName"></param>
    internal static void DeleteTask(string taskName)
    {
        try
        {
            var ts = new TaskService();
            ts.RootFolder.DeleteTask(taskName);
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception.Message);
        }
    }

    #endregion

    #region 注册表

    /// <summary>
    /// CreateFolderInLocalMachine
    /// </summary>
    /// <param name="targetPath"></param>
    internal static void CreateFolderInLocalMachine(string targetPath)
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
    internal static void DeleteFolderInLocalMachine(string targetPath)
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
    internal static bool SetupKeyValueInLocalMachine(string targetPath, string key,
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

    #endregion

    #region 应用

    /// <summary>
    /// Set boot to start automatically
    /// The setting is effective only when it is run in administrator mode
    /// </summary>
    /// <param name="taskName">Scheduled Task Name</param>
    /// <param name="exeFullName">Exe full path</param>
    /// <param name="desc">Description</param>
    /// <param name="delay">Delayed start time</param>
    /// <param name="used">Whether to boot automatically</param>
    public static bool AutoStart(string taskName, string exeFullName, string desc,
        TimeSpan delay, bool used = true)
    {
        if (used)
        {
            return AddTask(taskName, exeFullName, desc, delay);
        }

        DeleteTask(taskName);

        return true;
    }

    /// <summary>
    /// AllowEdgeSwipe
    /// </summary>
    /// <param name="status">Open or close</param>
    /// <returns></returns>
    public static bool AllowEdgeSwipe(bool status)
    {
        const string targetPath = "SOFTWARE\\Policies\\Microsoft\\Windows\\EdgeUI";
        CreateFolderInLocalMachine(targetPath);
        return SetupKeyValueInLocalMachine(targetPath,
            "AllowEdgeSwipe", status ? 1 : 0, RegistryValueKind.DWord);
    }

    #endregion
}