using System;
using System.Collections.Generic;
using System.Text;

namespace BatteryUtility.Models
{
    public class BatteryInfo
    {
        //Connection to AC power
        public string PowerStatus { get; set; }
        //Percentage of the battery
        public string BatteryPercentage { get; set; }
    }
}
