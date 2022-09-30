using System;
using System.Threading;
using System.Threading.Tasks;


namespace Common.Tool
{
    public class ContinousStarter
    {
        public bool IsEnabled { get; set; }


        readonly Action action;


        public ContinousStarter(Action action)
        {
            this.action = action;
            Task.Run(() => ContinousStarterMethod());
        }

        private void ContinousStarterMethod()
        {
            while(true)
            {
                if (IsEnabled)
                    action.Invoke();
                else
                    Thread.Sleep(100);
            }
        }
    }
}
