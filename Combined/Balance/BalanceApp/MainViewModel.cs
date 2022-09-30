using BalanceLib;
using System.IO;
using System.Windows;


namespace BalanceApp
{
    public class MainViewModel
    {
        public Balance Balance { get; }

        string configFileName = "id.cfg";

        //7C2819CB28A24092918400DCC8942852

        public MainViewModel()
        {
            string configFilePath = Path.Combine(Directory.GetCurrentDirectory(), configFileName);

            if(!File.Exists(configFilePath))
            {
                MessageBox.Show(configFileName + " not found!", "Error:", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown(-1);
            }
            else
            {
                string[] lines = File.ReadAllLines(configFilePath);

                Balance = new Balance();
                Balance.SerialNo = lines[0];
                Balance.ConnectionError += Balance_ConnectionError;
                Balance.Connect();
            }
        }

        private void Balance_ConnectionError()
        {
            MessageBox.Show("Connect failed!", "Error:", MessageBoxButton.OK, MessageBoxImage.Error);
            Application.Current.Shutdown(-1);
        }
    }
}