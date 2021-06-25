using BatteryUtility.Models;
using BatteryUtility.Utilities;
using Microsoft.Win32;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BatteryUtility
{
    class Program
    {
        private static BatteryUtil batteryUtility = new BatteryUtil();
        static void Main(string[] args)
        {
            BatteryInfo batteryInfo = batteryUtility.GetBatteryInfo();
            Console.WriteLine(batteryInfo.BatteryPercentage);

            batteryUtility.SubscribeBatteryEvents();
            batteryUtility.InitializeTimer();

            Console.ReadKey();
        }        
    }
}
