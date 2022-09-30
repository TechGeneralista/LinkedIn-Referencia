using System;
using System.Diagnostics;


namespace Common.Tool
{
    public class TimeMeasure
    {
        public static TimeMeasure StartNew()
        {
            TimeMeasure timeMeasure = new TimeMeasure();
            timeMeasure.Start();
            return timeMeasure;
        }


        public long ElapsedMs => stopwatch.ElapsedMilliseconds;


        Stopwatch stopwatch = new Stopwatch();


        public void Start() => stopwatch.Start();
        public void Restart() => stopwatch.Restart();

        public long Stop()
        {
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }

        public long Stop(string name)
        {
            stopwatch.Stop();
            Console.WriteLine(name + ": " + stopwatch.ElapsedMilliseconds);
            return stopwatch.ElapsedMilliseconds;
        }
    }
}
