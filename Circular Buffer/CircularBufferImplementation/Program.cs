using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircularBufferImplementation
{
    class Program
    {
        static void Main(string[] args)
        {
            CircularBuffer<KeyValuePair<ulong, ulong>> circularBuffer = new CircularBuffer<KeyValuePair<ulong, ulong>>(5);
            circularBuffer.Enqueue(new KeyValuePair<ulong, ulong>(1000, 1234));
            circularBuffer.Enqueue(new KeyValuePair<ulong, ulong>(2000, 2234));
            circularBuffer.Enqueue(new KeyValuePair<ulong, ulong>(3000, 3234));
            circularBuffer.Enqueue(new KeyValuePair<ulong, ulong>(4000, 4234));
            circularBuffer.Enqueue(new KeyValuePair<ulong, ulong>(5000, 5234));
            circularBuffer.Enqueue(new KeyValuePair<ulong, ulong>(6000, 6234));

            int i = 0;
            KeyValuePair<ulong, ulong> outputDict1 = new KeyValuePair<ulong, ulong>();
            while (i < 5)
            {
                outputDict1 = circularBuffer.Dequeue();
                Console.WriteLine(outputDict1);
                i++;
            }
            Console.WriteLine();

            circularBuffer.Enqueue(new KeyValuePair<ulong, ulong>(7000, 7234));
            circularBuffer.Enqueue(new KeyValuePair<ulong, ulong>(8000, 8234));
            circularBuffer.Enqueue(new KeyValuePair<ulong, ulong>(9000, 9234));

            i = 0;
            KeyValuePair<ulong, ulong> outputDict = new KeyValuePair<ulong, ulong>();
            List<ulong> dict = new List<ulong>();
            while (i < 5)
            {                
                outputDict = circularBuffer.Dequeue();
                dict.Add(outputDict.Value);
                //Console.WriteLine(outputDict);
                i++;
            }
            dict = Enumerable.Reverse(dict).Take(3).Reverse().ToList();
            //foreach (ulong key in dict.Keys)
            //{
            //    Console.WriteLine("Key: {0}, Value: {1}", key, dict[key]);
            //}

            Console.ReadKey();
        }
    }
}
