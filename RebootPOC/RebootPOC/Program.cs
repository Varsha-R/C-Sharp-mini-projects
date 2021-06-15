using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RebootPOC
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Setting flag using serviceossdk");
            string pathToDll = Path.Combine(Directory.GetCurrentDirectory(), "serviceossdk.dll");
            Assembly assembly = Assembly.LoadFrom(pathToDll);
            Type serviceOSSvcType = assembly.GetType("Dell.ServiceOS.ServiceOSSvc");
            //Type serviceOSSvcType = assembly.GetType("serviceossdk.ServiceOSSvc");
            object instanceOfServiceOSSvc = Activator.CreateInstance(serviceOSSvcType);
            try
            {                
                MethodInfo eraseSupportedMethod = serviceOSSvcType.GetMethod("IsSecureResetSupported", Type.EmptyTypes);
                bool eraseSupported = (bool)eraseSupportedMethod.Invoke(instanceOfServiceOSSvc, null);
                Console.WriteLine("Erase supported before catching exception: {0}", eraseSupported);
                //if (eraseSupported)
                //{
                //    Console.WriteLine("Erase supported");
                //    MethodInfo setRebootFlagMethod = serviceOSSvcType.GetMethod("SetNextBootToServiceOS", new Type[] { typeof(bool) });
                //    bool setRebootFlagSuccess = (bool)setRebootFlagMethod.Invoke(instanceOfServiceOSSvc, new object[] { true });
                //    if (setRebootFlagSuccess)
                //    {
                //        Console.WriteLine("Set reboot flag success");
                //        Console.WriteLine("Press a key to reboot");
                //        Console.ReadKey();
                //        RebootHelper.Reboot();
                //    }
                //    else
                //    {
                //        Console.WriteLine("FAILURE!");
                //        Console.ReadKey();
                //    }
                //}
                //else
                //{
                //    Console.WriteLine("Erase not supported! :(");
                //    Console.ReadKey();
                //}
                Console.ReadKey();
            }
            catch(TargetInvocationException ex)
            {
                Console.WriteLine("EXCEPTION :| {0}", ex.InnerException);
                if(ex.InnerException.ToString().Contains("ServiceOSAgentNotRunningException"))
                {
                    //Console.WriteLine("{0}. Starting service", ex.InnerException.ToString());
                    MethodInfo startServiceOSEngine = serviceOSSvcType.GetMethod("StartServiceOSEngine", Type.EmptyTypes);
                    bool serviceStarted = (bool)startServiceOSEngine.Invoke(instanceOfServiceOSSvc, null);
                    Console.WriteLine("Service started: {0}", serviceStarted);
                    if(serviceStarted)
                    {
                        Console.WriteLine("checking if erase supported");
                        MethodInfo eraseSupportedMethod = serviceOSSvcType.GetMethod("IsSecureResetSupported", Type.EmptyTypes);
                        try
                        {
                            bool eraseSupported = (bool)eraseSupportedMethod.Invoke(instanceOfServiceOSSvc, null);
                            Console.WriteLine("Erase supported: {0}", eraseSupported);
                        }
                        catch (Exception exc)
                        {
                            Console.WriteLine("Exception: {0}", exc);
                        }                        
                    }
                }
                Console.ReadKey();
            }
        } 
    }
}
