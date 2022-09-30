using System.Diagnostics;


namespace SmartVisionClientApp.Common
{
    public class TimeMeasure
    {
        Stopwatch stopwatch = new Stopwatch();

        public static TimeMeasure StartNew()
        {
            TimeMeasure timeMeasure = new TimeMeasure();
            timeMeasure.Start();
            return timeMeasure;
        }

        public void Start() => stopwatch.Restart();
        public long Stop()
        {
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }

        public long Stop(string name)
        {
            stopwatch.Stop();
            Debug.WriteLine(name + ": " + stopwatch.ElapsedMilliseconds);
            return stopwatch.ElapsedMilliseconds;
        }
    }
}
