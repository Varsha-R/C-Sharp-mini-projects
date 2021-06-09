using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Reflection;
using System.Xml.Linq;

namespace FirewallRule
{
    class Program
    {
        static void Main(string[] args)
        {
            string favoritesPath = Environment.GetFolderPath(Environment.SpecialFolder.Favorites);
            Console.WriteLine("HELLO"+Environment.UserName);
            Console.WriteLine(favoritesPath);
            Console.ReadKey();
        }
    }
}
