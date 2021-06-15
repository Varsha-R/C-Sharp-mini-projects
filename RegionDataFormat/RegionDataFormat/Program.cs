using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegionDataFormat
{
    class Program
    {
        static void Main(string[] args)
        {
            RegistryKey regionFormatData = Registry.CurrentUser.OpenSubKey(@"Control Panel\International", true);
            string[] valueNames = regionFormatData.GetValueNames();
            string calendarType = regionFormatData.GetValue("iCalendarType").ToString();
            Dictionary<string, string> shortLongDates = new Dictionary<string, string>();
            string exportRegionalDataFormat = @"D:\RegionalDataFormat.txt";
            if (calendarType.Equals("1"))
            {
                if (valueNames.Contains("sShortDate"))
                {
                    shortLongDates["sShortDate"] = "dd/MM/yyyy";
                }
                if (valueNames.Contains("sLongDate"))
                {
                    shortLongDates["sLongDate"] = "dddd, d MMMM yyyy";
                }
            }
            using (FileStream fsWDT = new FileStream(exportRegionalDataFormat, FileMode.Append, FileAccess.Write))
            using (StreamWriter swDT = new StreamWriter(fsWDT))
            {
                foreach (string dates in shortLongDates.Keys)
                {
                    swDT.Write(dates + ",|%|,");                    
                    swDT.Write(shortLongDates[dates] + ",|%|,");
                    swDT.Write("\n");
                }
            }

            string importRegionalDataFormat = @"D:\RegionalDataFormat.txt";
            string[] shortLongDatesImport = File.ReadAllLines(importRegionalDataFormat);
            Dictionary<string, string> newShortLongDates = new Dictionary<string, string>();
            foreach (string dates in shortLongDatesImport)
            {
                string[] split = dates.Split(new string[] { ",|%|," }, StringSplitOptions.None);
                newShortLongDates[split[0]] = split[1];
            }
            if(newShortLongDates.Keys.Contains("sShortDate"))
            {
                //set value
            }
            if (newShortLongDates.Keys.Contains("sLongDate"))
            {
                //set value
            }


            Console.ReadKey();
        }
    }
}
