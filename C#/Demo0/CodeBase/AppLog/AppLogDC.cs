using Common;
using Common.NotifyProperty;
using Language;
using System;
using System.Collections.Generic;
using System.IO;


namespace AppLog
{
    public class AppLogDC : ILog
    {
        public LanguageDC LanguageDC { get; }
        public INonSettableObservableProperty<AppLogMessage> LastMessage { get; } = new ObservableProperty<AppLogMessage>();
        public INonSettableObservableProperty<AppLogMessage[]> AllMessages { get; } = new ObservableProperty<AppLogMessage[]>(new AppLogMessage[0]);
        public INonSettableObservableProperty<AppLogMessage[]> ShowedAllMessages { get; } = new ObservableProperty<AppLogMessage[]>(new AppLogMessage[0]);
        public ISettableObservableProperty<bool> IsFreezed { get; } = new ObservableProperty<bool>();


        int maximumLogLength;


        public AppLogDC(LanguageDC languageDC, int maximumLogLength)
        {
            LanguageDC = languageDC;
            this.maximumLogLength = maximumLogLength;
            AllMessages.CurrentValueChanged += AllMessages_CurrentValueChanged;
            IsFreezed.CurrentValueChanged += IsFreezed_CurrentValueChanged;
        }

        ~AppLogDC()
        {
            List<string> logLines = new List<string>();
            foreach (AppLogMessage alm in AllMessages.CurrentValue)
                logLines.Add(alm.Message);

            string logsDir = "logs";
            string dirPath = Utils.GetPath(logsDir);

            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);

            string filePath = Utils.GetPath(logsDir, string.Format("{0}.txt", DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss")));
            File.WriteAllLines(filePath, logLines.ToArray());
        }

        private void IsFreezed_CurrentValueChanged(bool ov, bool nv)
        {
            if(!nv)
                ShowedAllMessages.ForceSet(AllMessages.CurrentValue);
        }

        private void AllMessages_CurrentValueChanged(AppLogMessage[] ov, AppLogMessage[] nv)
        {
            if (IsFreezed.CurrentValue)
                return;

            ShowedAllMessages.ForceSet(nv);
        }

        public void NewMessage(LogTypes logType, string message, string parameter = null)
        {
            AppLogMessage logMessage = new AppLogMessage(LanguageDC, logType, message, parameter);
            List<AppLogMessage> logMessages = new List<AppLogMessage>();
            logMessages.Add(logMessage);
            logMessages.AddRange(AllMessages.CurrentValue);

            if (logMessages.Count > maximumLogLength)
                logMessages.RemoveAt(logMessages.Count - 1);

            AllMessages.ForceSet(logMessages.ToArray());
            LastMessage.ForceSet(logMessage);
        }

        public void ShowLogView()
        {
            if (IsFreezed.CurrentValue)
                IsFreezed.CurrentValue = false;

            AppLogWindow appLogWindow = new AppLogWindow();
            appLogWindow.DataContext = this;
            appLogWindow.ShowDialog();
        }
    }
}
