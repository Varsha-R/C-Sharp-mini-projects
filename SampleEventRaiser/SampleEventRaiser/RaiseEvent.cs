using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace SampleEventRaiser
{
    public delegate void ProgressUpdatedHandler(double progress, double timeRemaining);
    public class RaiseEvent
    {
        public event ProgressUpdatedHandler OnProgressUpdated;
        private static System.Timers.Timer updateUITimer;
        public int i = 0;

        public void InvokeEvent()
        {
            Console.WriteLine("Invoked event. Started timer");
            updateUITimer = new System.Timers.Timer(5000);
            updateUITimer.Elapsed += UpdateUITimer_Elapsed;
            updateUITimer.AutoReset = true;
            updateUITimer.Enabled = true;
        }

        private void UpdateUITimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine("Timer elapsed in SampleEventRaiser");
            i += 2;
            double time = e.SignalTime.TimeOfDay.TotalMilliseconds;
            if(i == 100)
            {
                updateUITimer.Stop();
                updateUITimer.Dispose();
            }
            OnProgressUpdated?.Invoke(i, time);
        }
    }
}
