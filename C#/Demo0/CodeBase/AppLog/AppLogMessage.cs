using Common;
using Language;
using System;
using System.Windows.Media;


namespace AppLog
{
    public class AppLogMessage
    {
        public string Message { get; }
        public string ForegroundColor { get; }
        public string BackgroundColor { get; }


        readonly LanguageDC languageDC;


        public AppLogMessage(LanguageDC languageDC, LogTypes logType, string message, string parameter)
        {
            this.languageDC = languageDC;
            string typeString = null;

            switch(logType)
            {
                case LogTypes.Information:
                    typeString = string.Format("{0} ", languageDC.InformationColon.CurrentValue);
                    BackgroundColor = Colors.Green.ToString();
                    ForegroundColor = Colors.White.ToString();
                    break;

                case LogTypes.Successful:
                    typeString = string.Format("{0} ", languageDC.SuccessfulColon.CurrentValue);
                    BackgroundColor = Colors.Blue.ToString();
                    ForegroundColor = Colors.White.ToString();
                    break;

                case LogTypes.Warning:
                    typeString = string.Format("{0} ", languageDC.WarningColon.CurrentValue);
                    BackgroundColor = Colors.Orange.ToString();
                    ForegroundColor = Colors.Black.ToString();
                    break;

                case LogTypes.Error:
                    typeString = string.Format("{0} ", languageDC.ErrorColon.CurrentValue);
                    BackgroundColor = Colors.Red.ToString();
                    ForegroundColor = Colors.White.ToString();
                    break;
            }

            if(parameter.IsNull())
                Message = string.Format("{0} {1}{2}", DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss"), typeString, message);
            else
                Message = string.Format("{0} {1}{2} {3}", DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss"), typeString, message, parameter);
        }
    }
}