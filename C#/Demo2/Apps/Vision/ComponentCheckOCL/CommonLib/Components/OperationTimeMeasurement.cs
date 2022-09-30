using System;
using System.Diagnostics;


namespace CommonLib.Components
{
    public class OperationTimeMeasurement : ObservableProperty
    {
        public long OperationTimeMs
        {
            get => operationTimeMs;
            private set => SetField(value, ref operationTimeMs);
        }


        long operationTimeMs;
        Stopwatch stopwatch;


        public void StartOperation()
        {
            stopwatch = Stopwatch.StartNew();
        }

        public void EndOperation()
        {
            stopwatch.Stop();
            OperationTimeMs = stopwatch.ElapsedMilliseconds;
        }

        public void WriteToConsole(string message)
        {
            Console.WriteLine(message + ": " + stopwatch.ElapsedTicks + " ticks");
            Console.WriteLine(message + ": " + stopwatch.ElapsedMilliseconds + " ms");
            Console.WriteLine();
        }
    }
}
