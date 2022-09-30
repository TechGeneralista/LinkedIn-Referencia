using MultiCamApp.NetworkCommunication;
using System;
using System.ComponentModel;
using System.Net;
using System.Text;
using System.Windows;


namespace MultiCamApp.Main
{
    public class MainWindowViewModel
    {
        public WindowTitle WindowTitle { get; }
        public ImageDisplay ImageDisplay { get; }
        public Content Content { get; }


        public MainWindowViewModel()
        {
            if (DesignerProperties.GetIsInDesignMode(Application.Current.MainWindow))
                return;

            Application.Current.Resources[nameof(MainWindowViewModel)] = this;

            WindowTitle = new WindowTitle();
            ImageDisplay = new ImageDisplay();
            Content = new Content();
        }

        private void printMessage(IPEndPoint arg1, byte[] arg2)
        {
            Console.WriteLine(Encoding.ASCII.GetString(arg2));
        }
    }
}
