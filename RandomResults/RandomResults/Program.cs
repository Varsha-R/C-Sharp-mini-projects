using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomResults
{
    class Program
    {
        static void Main(string[] args)
        {
            byte[] hmac = { 162, 244, 37, 135, 24, 63, 34, 247, 57, 183, 218, 183, 210, 234, 240, 91 };
            Console.WriteLine(Convert.ToBase64String(hmac));
            Console.ReadKey();
        }
    }
}
