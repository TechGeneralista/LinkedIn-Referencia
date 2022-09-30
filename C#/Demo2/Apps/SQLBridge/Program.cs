using System;
using System.Net;
using System.Windows;


namespace SQLBridgeApp
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                MessageBox.Show("First param -> IP address\nSecond param -> port number", Constants.Error + ':', MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            IPAddress ipAddress = null;
            int portNumber = 0;

            try
            {
                ipAddress = IPAddress.Parse(args[0]);
                portNumber = int.Parse(args[1]);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Constants.Error + ':', MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            SQLBridge sqlBridge = new SQLBridge(ipAddress, portNumber);

            try
            {
                sqlBridge.Start();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, Constants.Error + ':', MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
