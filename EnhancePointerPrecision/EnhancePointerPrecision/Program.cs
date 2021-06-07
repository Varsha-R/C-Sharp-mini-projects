using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace EnhancePointerPrecision
{
    class Program
    {
        [DllImport("user32.dll", EntryPoint = "SystemParametersInfo", SetLastError = true)]
        public static extern bool SystemParametersInfo(uint action, uint uiParam, int[] pvParam, uint fWinIni);
        const int SPI_GETMOUSE = 0x0003;
        const int SPI_SETMOUSE = 0x0004;
        const int SPIF_SENDCHANGE = 0x02;

        [DllImport("User32.dll")]
        public static extern bool SystemParametersInfo(uint uiAction, uint uiParam, ref uint pvParam, uint fWinIni);

        [DllImport("User32.dll")]
        public static extern bool SystemParametersInfo(uint uiAction, uint uiParam, uint pvParam, uint fWinIni);
        const uint SPI_GETMOUSEWHEELROUTING = 0x201C;
        const uint SPI_SETMOUSEWHEELROUTING = 0x201D;

        static void Main(string[] args)
        {
            // Enhance pointer precision
            //int[] mouseSpeed = new int[3];
            //int[] setMouseSpeed = { 6, 10, 1 };
            //SystemParametersInfo(SPI_GETMOUSE, 0, mouseSpeed, SPIF_SENDCHANGE);
            //Console.WriteLine("The thresholds are {0},{1},{2}", mouseSpeed[0], mouseSpeed[1], mouseSpeed[2]);
            //SystemParametersInfo(SPI_SETMOUSE, 0, setMouseSpeed, SPIF_SENDCHANGE);
            //SystemParametersInfo(SPI_GETMOUSE, 0, mouseSpeed, SPIF_SENDCHANGE);
            //Console.WriteLine("The thresholds are {0},{1},{2}", mouseSpeed[0], mouseSpeed[1], mouseSpeed[2]);

            //Scroll inactivity
            uint boolVal = 99;
            uint routing = 2;
            SystemParametersInfo(SPI_GETMOUSEWHEELROUTING, 0, ref boolVal, SPIF_SENDCHANGE);
            Console.WriteLine("Value: {0}", boolVal);
            SystemParametersInfo(SPI_SETMOUSEWHEELROUTING, 0, routing, SPIF_SENDCHANGE);

            Console.ReadKey();
        }
    }
}
