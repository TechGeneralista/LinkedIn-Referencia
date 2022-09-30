using Common;
using Common.NotifyProperty;
using Language;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;


namespace CustomControl.PopupWindow
{
    public class PopupWindowDC : IPopupWindow
    {
        public INonSettableObservableProperty<float> Angle0 { get; } = new ObservableProperty<float>();
        public INonSettableObservableProperty<float> Angle1 { get; } = new ObservableProperty<float>();
        public INonSettableObservableProperty<float> Angle2 { get; } = new ObservableProperty<float>();
        public ISettableObservableProperty<string> Message { get; } = new ObservableProperty<string>();


        readonly LanguageDC languageDC;
        readonly Window owner;
        PopupWindowV popupWindowV;
        CancellationTokenSource cancellationTokenSource;


        public PopupWindowDC(LanguageDC languageDC, Window owner)
        {
            this.languageDC = languageDC;
            this.owner = owner;
        }

        public void Show(string message = null)
        {
            Utils.InvokeIfNecessary(() => 
            {
                owner.IsEnabled = false;
                popupWindowV = new PopupWindowV();
                popupWindowV.Owner = owner;
                popupWindowV.DataContext = this;
                popupWindowV.Show();
            });

            cancellationTokenSource = new CancellationTokenSource();
            Task.Run(() => RotateMethod(), cancellationTokenSource.Token);

            if (message.IsNull())
                Message.CurrentValue = languageDC.PleaseWait.CurrentValue;
            else
                Message.CurrentValue = message;
        }

        public void Close()
        {
            Utils.InvokeIfNecessary(() => 
            {
                if (popupWindowV.IsNotNull())
                {
                    owner.IsEnabled = true;
                    popupWindowV.Close();
                    popupWindowV = null;
                }
            });

            cancellationTokenSource.Cancel();
        }

        private void RotateMethod()
        {
            while (!cancellationTokenSource.IsCancellationRequested)
            {
                Angle0.ForceSet(Angle0.CurrentValue + 3);

                if (Angle0.CurrentValue > 360)
                    Angle0.ForceSet(Angle0.CurrentValue - 360);

                Angle1.ForceSet(Angle1.CurrentValue - 4);

                if (Angle1.CurrentValue < 0)
                    Angle1.ForceSet(Angle1.CurrentValue + 360);

                Angle2.ForceSet(Angle2.CurrentValue + 5);

                if (Angle2.CurrentValue > 360)
                    Angle2.ForceSet(Angle2.CurrentValue - 360);

                Thread.Sleep(20);
            }
        }
    }
}
