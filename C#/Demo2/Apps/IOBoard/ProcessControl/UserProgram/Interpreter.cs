using Common.Prop;
using IOBoard;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using UserProgram.Internals;


namespace UserProgram
{
    public enum InterpreterStates { Stopped, Running }


    public class Interpreter
    {
        public INonSettableObservableProperty<InterpreterStates> Status { get; } = new ObservableProperty<InterpreterStates>();
        public Parser Parser { get; }


        Task task;
        CancellationTokenSource cancellationTokenSource;
        Lexer lexer = new Lexer();


        public Interpreter(IOBoardClient ioBoardClient)
        {
            Parser = new Parser(ioBoardClient);
        }

        internal void Run(string value)
        {
            if(Status.Value == InterpreterStates.Stopped)
            {
                cancellationTokenSource = new CancellationTokenSource();
                task = Task.Run(() => InterpreterThread(value), cancellationTokenSource.Token);
            }
        }

        internal void Stop()
        {
            if (Status.Value == InterpreterStates.Running)
            {
                cancellationTokenSource.Cancel();

                while (Status.Value != InterpreterStates.Stopped)
                    Thread.Sleep(10);
            }
        }

        private void InterpreterThread(string value)
        {
            Status.ForceSet(InterpreterStates.Running);

            try
            {
                lexer.CreateTokens(value);
                Parser.Initialize(lexer.CodeLines, lexer.Tokens);

                while (!cancellationTokenSource.IsCancellationRequested)
                    Parser.Loop();
            }

            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show(Application.Current.MainWindow, ex.Message, "Hiba:", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }

            cancellationTokenSource = null;
            task = null;
            Status.ForceSet(InterpreterStates.Stopped);
        }
    }
}