using System;



namespace IOBoardServer
{
    public class ConsoleLog : ILog
    {
        public event Action<string> ErrorOccurred;


        ConsoleColor defaultColor;


        public ConsoleLog()
        {
            defaultColor = Console.ForegroundColor;
        }

        public void NewMessage(LogLevels logLevel, string message)
        {
            switch(logLevel)
            {
                case LogLevels.Information:
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("Information: ");
                    Console.ForegroundColor = defaultColor;
                    Console.WriteLine(message);
                    break;

                case LogLevels.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("Warning: ");
                    Console.ForegroundColor = defaultColor;
                    Console.WriteLine(message);
                    break;

                case LogLevels.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("Error: ");
                    Console.ForegroundColor = defaultColor;
                    Console.WriteLine(message);
                    ErrorOccurred?.Invoke(message);
                    break;
            }
        }
    }
}
