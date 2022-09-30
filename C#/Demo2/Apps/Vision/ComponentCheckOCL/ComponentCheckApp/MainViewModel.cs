using CommonLib.Components;
using ComponentCheckApp.Views.DeviceSelect;
using D4I4OLib;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;


namespace ComponentCheckApp
{
    public class MainViewModel : ObservableProperty
    {
        public bool StartButtonIsEnable 
        {
            get => startButtonIsEnable;
            set => SetField(value, ref startButtonIsEnable);
        }

        public long Elapsedmilliseconds
        {
            get => elapsedmilliseconds;
            set => SetField(value, ref elapsedmilliseconds);
        }

        public int ProgressBarValue
        {
            get => progressBarValue;
            private set => SetField(value, ref progressBarValue);
        }

        public int ProgressBarMaximum
        {
            get => progressBarMaximum;
            private set => SetField(value, ref progressBarMaximum);
        }

        public int ProgressPercentage
        {
            get => progressPercentage;
            private set => SetField(value, ref progressPercentage);
        }

        public Visibility ProgressBarVisibility
        {
            get => progressBarVisibility;
            private set => SetField(value, ref progressBarVisibility);
        }


        bool startButtonIsEnable;
        long elapsedmilliseconds;
        Stopwatch stopwatch;
        int progressBarValue, progressBarMaximum, progressPercentage;
        Visibility progressBarVisibility;


        public MainViewModel()
        {
            Application.Current.Resources[ResourceKeys.MainViewModelKey] = this;
            progressBarVisibility = Visibility.Hidden;

            D4I4O d4I4O = (D4I4O)Application.Current.Resources[ResourceKeys.D4I4OKey];
            d4I4O.In0RisingEvent += Start;
        }

        public Task StartAsync() => Task.Run(() => Start());

        public void Start()
        {
            if (!startButtonIsEnable)
                return;

            stopwatch = Stopwatch.StartNew();
            StartButtonIsEnable = false;
            ProgressBarVisibility = Visibility.Visible;
            ((DeviceSelectViewModel)Application.Current.Resources[ResourceKeys.DeviceSelectViewModelKey]).Start();
        }

        public void End()
        {
            if(stopwatch != null)
            {
                stopwatch.Stop();
                Elapsedmilliseconds = stopwatch.ElapsedMilliseconds;
            }

            ProgressBarVisibility = Visibility.Hidden;
            StartButtonIsEnable = true;
        }

        public void SetProgressBar(int value, int maximum)
        {
            ProgressBarMaximum = maximum;
            ProgressBarValue = value;
            ProgressPercentage = (int)Math.Round(((double)value/(double)maximum)*100d);
        }

        public void HelpButtonClick()
        {
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = "doc.pdf";
                process.Start();
            }

            catch
            {
                MessageBox.Show("Can't open doc.pdf", "Error:", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}