using Microsoft.Win32;
using System;
using System.Runtime.InteropServices;

namespace CursorBlinkRateDemo
{
    class Program
    {
        const int SPI_GETMOUSE = 0x0003;
        const int SPIF_SENDCHANGE = 0x02;
        const int SPI_SETMOUSE = 0x0004;
        static int[] mouseSpeed = new int[3];

        [DllImport("user32.dll", EntryPoint = "SystemParametersInfo", SetLastError = true)]
        public static extern bool SystemParametersInfo(uint action, uint uiParam, int[] pvParam, uint fWinIni);

        [DllImport("user32")]
        internal static extern bool SetCaretBlinkTime(uint wMSeconds);
        static void Main(string[] args)
        {
            //Computer\HKEY_CURRENT_USER\Control Panel\Desktop
            uint cursorBlinkRate = 600;
            RegistryKey blinkRate = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
            //blinkRate.SetValue(@"CursorBlinkRate", cursorBlinkRate);
            //SetCaretBlinkTime(cursorBlinkRate);

            SystemParametersInfo(SPI_GETMOUSE, 0, mouseSpeed, SPIF_SENDCHANGE);
            Console.ReadKey();
        }
    }
}
