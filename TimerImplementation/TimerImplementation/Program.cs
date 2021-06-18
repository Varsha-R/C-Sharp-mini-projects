using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace TimerImplementation
{
    class Program
    {
        private static Timer aTimer;

        public static void Main()
        {
            #region Timer implementation
            //SetTimer();
            //Console.WriteLine("\nPress the Enter key to exit the application...\n");
            //Console.WriteLine("The application started at {0:HH:mm:ss.fff}", DateTime.Now);
            //Console.ReadLine();
            //aTimer.Stop();
            //aTimer.Dispose();
            //Console.WriteLine("Terminating the application...");
            #endregion

            DateTime currentTime = DateTime.Now;
            Console.WriteLine("Current time: {0}", currentTime);
            for(int i=0; i<1000; i++)
            {

            }
            DateTime after = DateTime.Now;
            Console.WriteLine("After: {0}", after);

            TimeSpan timeDifference = after - currentTime;
            long inMilliseconds = timeDifference.Milliseconds;
            double total = timeDifference.TotalMilliseconds;
            Console.WriteLine("Difference in milliseconds: {0}", inMilliseconds);
            Console.ReadKey();
        }

        private static void SetTimer()
        {
            // Create a timer with a two second interval.
            aTimer = new Timer(2000);
            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }

        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            Console.WriteLine("The Elapsed event was raised at {0:HH:mm:ss.fff}",
                              e.SignalTime);
        }
    }
}
