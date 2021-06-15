using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestWrapper
{
    class Program
    {
        static void Main(string[] args)
        {
            EraseWrapper.EraseWrapper eraseWrapper = new EraseWrapper.EraseWrapper();
            eraseWrapper.ProgressChangeEvent += EraseWrapper_ProgressChangeEvent;
            eraseWrapper.StartEvent();
            Console.ReadKey();
        }

        private static void EraseWrapper_ProgressChangeEvent(double progress, double timeRemaining)
        {
            TimeSpan timeRemainingInFormat = TimeSpan.FromMilliseconds(timeRemaining);
            Console.Write("Progress: {0}\t", Math.Round(progress, 2));
            Console.Write("Time remaining: {0}\n", timeRemainingInFormat.ToString(@"hh\:mm\:ss"));
        }
    }
}
