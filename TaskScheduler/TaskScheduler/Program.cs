using Microsoft.Win32.TaskScheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskScheduler
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {

                // Create a new task definition and assign properties
                TaskDefinition td = TaskService.Instance.NewTask();
                td.RegistrationInfo.Description = "Remind user to erase PC";
                //td.Triggers.Add(new BootTrigger()
                //{
                //    Enabled = true
                //});
                td.Triggers.Add(new LogonTrigger()
                {
                    Enabled = true,
                    EndBoundary = DateTime.Now + TimeSpan.FromDays(7)
                });

                //td.Triggers.Add(new TimeTrigger()
                //{
                //    StartBoundary = DateTime.Now + TimeSpan.FromSeconds(120),
                //    Enabled = true,
                //    EndBoundary = DateTime.Now + TimeSpan.FromSeconds(300),
                //});
                // Create a trigger that will fire the task at this time every other day
                //DailyTrigger dt = (DailyTrigger)td.Triggers.Add(new DailyTrigger()
                //                                                {
                //                                                    StartBoundary = DateTime.Now + TimeSpan.FromSeconds(10),
                //                                                    Enabled = true,
                //                                                    DaysInterval = 1,
                //                                                    Repetition = new RepetitionPattern(TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(15), stopAtDurationEnd: true)
                //                                                });

                // Create an action that will launch Notepad whenever the trigger fires
                td.Actions.Add(new ExecAction(@"C:\Projects\Notifier\Notification-8.1.exe", null, null));

                td.Settings.StopIfGoingOnBatteries = false;
                td.Settings.DisallowStartIfOnBatteries = false;
                td.Settings.StartWhenAvailable = true;

                td.Principal.UserId = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                td.Principal.LogonType = TaskLogonType.InteractiveToken;
                //td.Principal.RunLevel = TaskRunLevel.Highest;
                // Register the task in the root folder
                const string taskName = "TestTaskScheduling";
                TaskService.Instance.RootFolder.RegisterTaskDefinition(taskName, td);

                //Modify task
                //TaskDefinition getTd = TaskService.Instance.GetTask("TestTaskScheduling").Definition;
                //getTd.Triggers.Add(new TimeTrigger()
                //{
                //    StartBoundary = DateTime.Now + TimeSpan.FromSeconds(100),
                //    Enabled = true,
                //    EndBoundary = DateTime.Now + TimeSpan.FromSeconds(200)
                //});
                //TaskService.Instance.RootFolder.RegisterTaskDefinition(@"TestTaskScheduling", getTd);

                ////Delete task
                //using (TaskService ts = new TaskService())
                //{
                //    if (ts.GetTask("TestTaskScheduling") != null)
                //    {
                //        ts.RootFolder.DeleteTask("TestTaskScheduling");
                //    }
                //}

                //// Create a trigger that will fire after the system boot
                //getTd.Triggers.Add(new BootTrigger()
                //{                    
                //    Enabled = true,
                //    EndBoundary = DateTime.Now + TimeSpan.FromSeconds(200)
                //});

                //const string taskName1 = "TestTaskScheduling";
                //Microsoft.Win32.TaskScheduler.Task t = TaskService.Instance.AddTask(taskName1,
                //  new TimeTrigger()
                //  {
                //      StartBoundary = DateTime.Now + TimeSpan.FromSeconds(30),
                //      Enabled = true
                //  },
                //  new ExecAction(@"C:\Users\Varsha.Ravindra\source\repos\NotificationPOC4.5\NotificationPOC4.5\bin\Debug\NotificationPOC4.5.exe"));

                //const string taskName1 = "TestTaskScheduling";
                //Microsoft.Win32.TaskScheduler.Task t = TaskService.Instance.AddTask(
                //  taskName1,
                //  new DailyTrigger()
                //  {
                //      StartBoundary = DateTime.Now + TimeSpan.FromSeconds(30),
                //      Enabled = true,
                //      DaysInterval = 1,
                //      Repetition = new RepetitionPattern(TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(15), stopAtDurationEnd: true)
                //  },
                //  new ExecAction(@"C:\Users\Varsha.Ravindra\source\repos\NotificationPOC4.5\NotificationPOC4.5\bin\Debug\NotificationPOC4.5.exe"));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Caught exception: {0}", ex);
            }
            Console.WriteLine("Task scheduled");
            Console.ReadKey();
        }
    }
}
