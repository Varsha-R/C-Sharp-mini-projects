using Microsoft.Win32.TaskScheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserStoryNotificationsPOC
{
    public class TaskScheduler
    {
        public void ScheduleTask(string hours)
        {
            try
            {
                Console.WriteLine("Scheduling to be notified in 1 minute");
                // Create a new task definition and assign properties
                TaskDefinition td = TaskService.Instance.NewTask();
                td.RegistrationInfo.Description = "Remind user to erase PC";

                if (hours.Equals(null))
                {
                    td.Triggers.Add(new LogonTrigger()
                    {
                        Enabled = true,
                        Delay = TimeSpan.FromMinutes(2)
                    });
                }
                else
                {
                    td.Triggers.Add(new TimeTrigger()
                    {
                        StartBoundary = DateTime.Now + TimeSpan.FromMinutes(Convert.ToDouble(hours)),
                        Enabled = true,
                        EndBoundary = DateTime.Now + TimeSpan.FromMinutes(Convert.ToDouble(hours)) + TimeSpan.FromMinutes(3),
                    });
                }

                // Create an action that will launch Notepad whenever the trigger fires
                string path = System.Reflection.Assembly.GetExecutingAssembly().Location;
                td.Actions.Add(new ExecAction(path, null, null));

                td.Settings.StopIfGoingOnBatteries = false;
                td.Settings.DisallowStartIfOnBatteries = false;
                td.Settings.StartWhenAvailable = true;

                td.Principal.UserId = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                td.Principal.LogonType = TaskLogonType.InteractiveToken;
                td.Principal.RunLevel = TaskRunLevel.Highest;

                // Register the task in the root folder
                const string taskName = "TestTaskScheduling";
                TaskService.Instance.RootFolder.RegisterTaskDefinition(taskName, td);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Caught exception: {0}", ex);
            }
        }
    }
}
