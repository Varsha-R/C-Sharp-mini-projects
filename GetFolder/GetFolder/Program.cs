using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetFolder
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\Mozilla\\Firefox\\Profiles";
            Console.WriteLine(path);
            Console.WriteLine(Directory.GetAccessControl(path));
            if(Directory.Exists(path))
            {
                Console.WriteLine("Exists");
            }
            else
            {
                Console.WriteLine("Does not exist");
            }
            string defaultFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Mozilla\\Firefox\\Profiles");
            Console.WriteLine(defaultFolder);
            Console.ReadKey();
        }
    }
}
