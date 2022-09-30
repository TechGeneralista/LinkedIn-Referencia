using Common.Communication.TCP5;
using Common.Language;
using System.Windows;


namespace Common.License
{
    public class ApplicationClient : TCPClient
    {
        readonly LanguageDC languageDC;
        readonly AppInfo appInfo;
        readonly Window mainWindow;


        public ApplicationClient(LanguageDC languageDC, AppInfo appInfo, Window mainWindow, string host, int port, int packetBufferSize, int dataBufferSize, int receiveTimeout, string dataEnd, string arraySeparator)
            : base(host, port, packetBufferSize, dataBufferSize, receiveTimeout, dataEnd, arraySeparator)
        {
            this.languageDC = languageDC;
            this.appInfo = appInfo;
            this.mainWindow = mainWindow;
        }

        /// <summary>
        /// Csatlakozás a szerverhez
        /// </summary>
        /// <returns>siker esetén false, hiba esetén true</returns>
        public new bool Connect()
        {
            base.Connect();

            if (Status.Value == TCPClientStatus.Connected)
            {
                SendTextArray(appInfo.ApplicationName, appInfo.MajorVersion.ToString(), appInfo.MinorVersion.ToString(), appInfo.BuildVersion.ToString(), Utils.WMIInfo("Win32_DiskDrive", "SerialNumber").Trim());
                ReceiveText();
                return false;
            }

            Utils.InvokeIfNecessary(() => 
            {
                MessageBox.Show(mainWindow, languageDC.CantConnectToServer.Value, languageDC.ErrorColon.Value, MessageBoxButton.OK, MessageBoxImage.Error);
            });
            
            return true;
        }
    }
}
