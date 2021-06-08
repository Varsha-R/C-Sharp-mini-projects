using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstimatedTimeSectorWritePOC
{
    class Program
    {
        static float timeRequiredForOne = 0.05f;
        static long totalCount = 10000;
        static double totalETA = timeRequiredForOne * totalCount;

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
            Console.Write(progressPercent.ToString() + "%\t");
            double timeElapsed = progress * timeRequiredForOne;
            double ETA = totalETA - timeElapsed;
            Console.Write("Time Remaining: {0}\t", TimeSpan.FromSeconds(ETA).ToString(@"hh\:mm\:ss"));
            //Console.Write(progress.ToString() + " of " + total.ToString() + "    "); //blanks at the end remove any excess
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Total ETA: {0}", totalETA);
            TimeSpan timeSpan = TimeSpan.FromSeconds(totalETA);
            Console.WriteLine("Total Estimated Time: {0} s", timeSpan.ToString(@"hh\:mm\:ss"));
            for (int i = 0; i <= totalCount; i++)
            {
                DrawTextProgressBar(i, totalCount);
                Console.Write("\r");
            }
            Console.ReadKey();
        }

        //static void Main(string[] args)
        //{
        //    Console.WriteLine("Starting create file");

        //    //FileStream fs = new FileStream(@"D:\Rebit - Wipe\Dummy\dummy.txt", FileMode.CreateNew);
        //    //fs.Seek(10 * 1024 * 1024, SeekOrigin.Begin);
        //    //fs.WriteByte(0);
        //    //fs.Close();
            
        //    string fileName = @"D:\Rebit - Wipe\Dummy\dummy.txt";
        //    if (File.Exists(fileName))
        //    {
        //        Console.WriteLine("File already exists. Deleting file.");
        //        File.Delete(fileName);
        //    }
        //    byte[] dataArray = new byte[314572800]; //300 MB
        //    new Random().NextBytes(dataArray);

        //    Stopwatch stopWatch = new Stopwatch();            
        //    using (FileStream fileStream = new FileStream(fileName, FileMode.Create))
        //    {
        //        stopWatch.Start();
        //        for (int i = 0; i < dataArray.Length; i++)
        //        {
        //            fileStream.WriteByte(dataArray[i]);
        //        }
        //        stopWatch.Stop();
        //    }
            
        //    TimeSpan ts = stopWatch.Elapsed; 
        //    string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}:{3:00}",
        //        ts.Hours, ts.Minutes, ts.Seconds,
        //        ts.Milliseconds / 10);
        //    Console.WriteLine("Time taken for writing 300MB " + elapsedTime);

        //    Console.WriteLine("Ending create file");
        //    Console.ReadKey();
        //}

        //private void CreateDummyFile()
        //{

        //}
    }
}
