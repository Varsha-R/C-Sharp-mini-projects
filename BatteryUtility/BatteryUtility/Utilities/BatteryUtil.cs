using BatteryUtility.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;

namespace BatteryUtility.Utilities
{
    public class BatteryUtil
    {
        private System.Threading.Timer batteryTimer;
        private int previousBatteryStatus;
        private readonly SemaphoreSlim batteryStatusSemaphore = new SemaphoreSlim(1, 1);

        public BatteryInfo GetBatteryInfo()
        {
            // For SystemInformationHelper .csproj file should have <Project Sdk="Microsoft.NET.Sdk.WindowsDesktop">
            int powerStatus = (int)SystemInformation.PowerStatus.PowerLineStatus;
            float batteryLife = SystemInformation.PowerStatus.BatteryLifePercent;
            batteryLife *= 100;
            BatteryInfo batteryInfo = new BatteryInfo
            {
                PowerStatus = powerStatus.ToString(CultureInfo.InvariantCulture),
                BatteryPercentage = batteryLife.ToString(CultureInfo.InvariantCulture)
            };
            return batteryInfo;
        }

        public Tuple<string, int> GetBattery()
        {
            BatteryInfo batteryInfo = GetBatteryInfo();
            Console.WriteLine("Battery %: " + batteryInfo.BatteryPercentage);
            if (batteryInfo.PowerStatus.Equals("1", StringComparison.InvariantCultureIgnoreCase))
            {
                return new Tuple<string, int>("User PC Connected to AC Power Source", 0);
            }
            else if (batteryInfo.PowerStatus.Equals("0", StringComparison.InvariantCultureIgnoreCase) && Convert.ToDouble(batteryInfo.BatteryPercentage, CultureInfo.InvariantCulture) >= 20)
            {
                return new Tuple<string, int>("System is not plugged to AC Power source", 2464);
            }
            else
            {
                return new Tuple<string, int>("Battery Percentage is at a critical level", 2465);
            }
        }

        public void InitializeTimer()
        {
            float.TryParse(GetBatteryInfo()?.BatteryPercentage, out float batteryPercentage);
            batteryTimer = (batteryPercentage == 100)
                ? new System.Threading.Timer(BatteryTimerCallback, null, 0, 300000)
                : new System.Threading.Timer(BatteryTimerCallback, null, 0, 60000);
        }

        private async void BatteryTimerCallback(object state)
        {
            await Task.Run(() =>
            {
                PowerModeChanged(this, new PowerModeChangedEventArgs(PowerModes.StatusChange));
            });
        }

        public void SubscribeBatteryEvents()
        {
            SystemEvents.PowerModeChanged += new PowerModeChangedEventHandler(PowerModeChanged);
        }

        public void PowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            switch (e.Mode)
            {
                case PowerModes.StatusChange:
                    SendBatteryMessage();
                    break;
            }
        }

        private void SendBatteryMessage()
        {
            batteryStatusSemaphore.Wait();
            Tuple<string, int> battery = GetBattery();
            if (battery.Item2 == 2465 && battery.Item2 != previousBatteryStatus)
            {
                Console.WriteLine("Battery % at critial level"); // Send battery is critical
            }
            else if (battery.Item2 == 0 && previousBatteryStatus == 2465 && battery.Item2 != previousBatteryStatus)
            {
                Console.WriteLine("User PC plugged into AC power");
            }
            previousBatteryStatus = battery.Item2;
            batteryStatusSemaphore.Release();
        }

        // Disposing - will need to dispose battery timer
        // Unsubscription needs to be done at the right place too
        public void UnsubscribeBatteryEvents()
        {
            SystemEvents.PowerModeChanged -= new PowerModeChangedEventHandler(PowerModeChanged);
        }
    }
}
