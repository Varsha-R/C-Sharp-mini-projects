using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonMatch
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> old = new List<string>
            {
                "varsha.ravindra@outlook.com",
                "varsha.ravindra@xyz.com"
            };

            List<string> newS = new List<string>
            {
                "varsha.ravindra@xyz.com"
            };

            IEnumerable<string> commons = Enumerable.Empty<string>();
            commons = old.Intersect(newS);
            foreach(string common in commons)
            {
                Console.WriteLine(common);
            }
            Console.ReadKey();
        }
    }
}
