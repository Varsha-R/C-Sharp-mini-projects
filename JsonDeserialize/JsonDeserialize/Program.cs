using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JsonDeserialize
{
    class Program
    {
        [DllImport("User32.dll")]
        internal static extern bool SystemParametersInfo(uint uiAction, uint uiParam, ref uint pvParam, uint fWinIni);

        const int SPI_SETCURSORS = 0x0057;

        const int SPIF_UPDATEINIFILE = 0x01;

        const int SPIF_SENDCHANGE = 0x02;
        static void Main(string[] args)
        {
            RegistryKey mouseKey = Registry.CurrentUser.OpenSubKey(@"Control Panel\Cursors", true);
            mouseKey.SetValue("Person", @"C:\Users\Varsha.Ravindra\AppData\Local\Microsoft\Windows\Cursors\person_eoa.cur", RegistryValueKind.ExpandString);
            uint _pvParam = 0;
            bool success = SystemParametersInfo(SPI_SETCURSORS, 0, ref _pvParam, SPIF_SENDCHANGE);
                                 
        //try
        //{
        //    string info = File.ReadAllText(@"C:\Users\Varsha.Ravindra\Documents\Registry\Empty\info.txt");
        //    //JObject json = JObject.Parse(str);
        //    Console.WriteLine(info);
        //    SettingsInformation settings = JsonConvert.DeserializeObject<SettingsInformation>(info);
        //    if (settings.Version.Equals("1.0.0.0"))
        //    {
        //        Console.WriteLine( settings.Settings);
        //    }
        //    Console.WriteLine(settings.ToString());
        //    Console.ReadKey();
        //}
        //catch (Exception exception)
        //{
        //    Console.WriteLine(exception);
        //}
    }
}
}
