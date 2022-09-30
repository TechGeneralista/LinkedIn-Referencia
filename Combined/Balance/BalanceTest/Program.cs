using BalanceLib;
using System;
using System.Threading;

namespace BalanceTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Balance balance = new Balance();
            balance.SerialNo = "7C2819CB28A24092918400DCC8942852";
            balance.Connect();

            while(true)
            {
                Console.WriteLine("Weight: " + balance.MeasuredWeight + "kg");

                if(Console.KeyAvailable)
                {
                    ConsoleKeyInfo keyInfo = Console.ReadKey();

                    if (keyInfo.Key == ConsoleKey.Spacebar)
                        break;
                }

                Thread.Sleep(100);
            }
        }
    }
}
