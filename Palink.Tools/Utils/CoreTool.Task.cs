using System;
using System.Linq;
using Microsoft.Win32.TaskScheduler;

namespace Palink.Tools.Utils;

/// <summary>
/// CoreTool
/// </summary>
public partial class CoreTool
{
    /// <summary>
    /// 添加计划任务
    /// </summary>
    /// <param name="taskName"></param>
    /// <param name="exeFullName"></param>
    /// <param name="description"></param>
    /// <param name="delay"></param>
    /// <returns></returns>
    public static bool AddTask(string taskName, string exeFullName, string description,
        TimeSpan delay)
    {
        try
        {
            if (ExitTask(taskName, description, delay))
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
    public static bool ExitTask(string taskName, string description, TimeSpan delay)
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
    public static void DeleteTask(string taskName)
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
}