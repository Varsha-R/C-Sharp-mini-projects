using System;

namespace SectorWriteProgress
{
    class ProgressBar
    {
        private int _lastOutputLength;
        private readonly int _maximumWidth;

        public ProgressBar(int maximumWidth)
        {
            _maximumWidth = maximumWidth;
            Show(" [ ");
        }

        public void Update(double percent)
        {
            // Remove the last state           
            string clear = string.Empty.PadRight(_lastOutputLength, '\b');
            Show(clear);

            // Generate new state           
            try
            {
                int width = (int)(percent / 100 * _maximumWidth);
                int fill = _maximumWidth - width;
                string output = string.Format("{0}{1} ] {2}%", string.Empty.PadLeft(width, '='), string.Empty.PadLeft(fill, ' '), percent.ToString("0.0"));
                Show(output);
                _lastOutputLength = output.Length;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void Show(string value)
        {
            Console.Write(value);
        }

        public static void DrawTextProgressBar(int progress, long total)
        {
            //draw empty progress bar
            Console.CursorVisible = false;
            Console.CursorLeft = 0;
            Console.Write("["); //start
            Console.CursorLeft = 32;
            Console.Write("]"); //end
            Console.CursorLeft = 1;
            float onechunk = 30.0f / total;

            //draw filled part
            int position = 1;
            for (int i = 0; i < onechunk * progress; i++)
            {
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.CursorLeft = position++;
                Console.Write(" ");
            }

            //draw unfilled part
            for (int i = position; i <= 31; i++)
            {
                Console.BackgroundColor = ConsoleColor.Green;
                Console.CursorLeft = position++;
                Console.Write(" ");
            }

            //draw totals
            Console.CursorLeft = 35;
            Console.BackgroundColor = ConsoleColor.Black;

            float progressPercent = (int)(((float)progress / total) * 100);
            Console.Write(progressPercent.ToString() + "%");
            //Console.Write(progress.ToString() + " of " + total.ToString() + "    "); //blanks at the end remove any excess
        }

        public static void Main()
        {
            long totalCount = 100;
            for (int i = 0; i <= totalCount; i++)
            {
                DrawTextProgressBar(i, totalCount);
                Console.Write("\r");
            }
            Console.ReadKey();
            //var progress = new ProgressBar(100);
            //for (int i = 0; i < 100000; i++)
            //{
            //    progress.Update(i);
            //    System.Threading.Thread.Sleep(50);
            //}
            //Console.WriteLine();
        }
    }
}
