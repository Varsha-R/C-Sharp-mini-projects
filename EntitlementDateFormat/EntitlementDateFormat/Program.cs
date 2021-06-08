using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntitlementDateFormat
{
    class Program
    {
        static void Main(string[] args)
        {
            string decryptedString = "N62020-09-10T15:08:344z";
            string endDateWithTimeZone = decryptedString.Substring(decryptedString.Length - 21);
            string endDateWithoutTimeZone = endDateWithTimeZone.Substring(0, endDateWithTimeZone.Length - 2);
            endDateWithoutTimeZone = endDateWithoutTimeZone.Replace("T", " ");

            string now = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
            DateTime today = DateTime.ParseExact(now, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
            DateTime endDate = DateTime.ParseExact(endDateWithoutTimeZone, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

            if(today < endDate)
            {
                Console.WriteLine("Entitled");
            }
            else
            {
                Console.WriteLine("Not entitled");
            }

            Console.ReadKey();
        }
    }
}
