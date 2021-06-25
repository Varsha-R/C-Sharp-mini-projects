using NetworkUtility.Utilities;
using System;

namespace NetworkUtility
{
    class Program
    {
        private static NetworkUtil networkUtil = new NetworkUtil();
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            networkUtil.SubscribeNetworkEvents();
            bool isIT = networkUtil.GetNetworkStatus();
            Console.WriteLine("Is it connected to internet? " + isIT);
            Console.ReadKey();
        }
    }
}
