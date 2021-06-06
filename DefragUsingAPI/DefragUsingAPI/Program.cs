using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Management;

namespace DefragAPI
{
    // Refer System.Management
    // Project Properties -> Build -> Platform Target: Any CPU <uncheck all other options>
    class Program
    {
        static void Main(string[] args)
        {

            ManagementScope scope = new ManagementScope("\\\\.\\ROOT\\Microsoft\\Windows\\Storage");
            ObjectQuery query = new ObjectQuery("SELECT * FROM MSFT_Volume");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);
            ManagementObjectCollection queryCollection = searcher.Get();

            foreach (ManagementObject m in queryCollection)
            {
                try
                {
                    ManagementBaseObject inParams = m.GetMethodParameters("Optimize");

                    //PropertyDataCollection systemPropertyData = inParams.SystemProperties;
                    //foreach(PropertyData data in systemPropertyData)
                    //{
                    //    Console.WriteLine("{0}: {1}", data.Name, data.Value);
                    //}
                    //PropertyDataCollection propertyData = inParams.Properties;
                    //foreach (PropertyData data in propertyData)
                    //{
                    //    Console.WriteLine("{0}: {1}", data.Name, data.Value);
                    //}

                    PropertyDataCollection properties = inParams.Properties;
                    List<string> propertyNames = new List<string>();
                    foreach(PropertyData property in properties)
                    {
                        propertyNames.Add(property.Name);
                    }
                    if (propertyNames.Contains("ReTrim"))
                    {
                        Console.WriteLine("Optimize contains retrim");
                        inParams.SetPropertyValue("ReTrim", true);
                        ManagementBaseObject outParams = m.InvokeMethod("Optimize", inParams, null);
                        Console.WriteLine("ReturnValue: {0}", outParams["ReturnValue"]);
                        if (Convert.ToInt32(outParams["ReturnValue"]) == 0)
                        {
                            Console.WriteLine("Trimming of drive {0} complete", m["DriveLetter"]);
                        }
                        else
                        {
                            PropertyDataCollection outParamsProperties = outParams.Properties;
                            foreach(PropertyData property in outParamsProperties)
                            {
                                if(property.Name.Equals("ExtendedStatus"))
                                {
                                    Console.WriteLine("---Extended status---");
                                    ManagementBaseObject extendedStatus = (ManagementBaseObject)property.Value;
                                    PropertyDataCollection systemPropertyData = extendedStatus.Properties;
                                    foreach (PropertyData data in systemPropertyData)
                                    {
                                        Console.WriteLine("{0}: {1}", data.Name, data.Value);
                                    }
                                }
                            }
                            Console.WriteLine("Can't perform trim on this. FileSystemLabel: {0}", m["FileSystemLabel"]);
                            Console.WriteLine("Trimming of drive: {0} unsuccessful", m["DriveLetter"]);
                        }
                    }
                    else
                    {
                        Console.WriteLine("ReTrim property does not exist");
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Can't trim this.{0}", ex);
                }
                Console.WriteLine();
            }
            Console.ReadKey();
        }
    }
}
