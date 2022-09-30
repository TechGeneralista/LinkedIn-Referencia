using Common.Language;
using Common.NotifyProperty;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;


namespace Common.PopupWindow
{
    public class PopupWindowDC : IPopupWindow
    {
        public IReadOnlyProperty<float> Angle0 { get; } = new Property<float>();
        public IReadOnlyProperty<float> Angle1 { get; } = new Property<float>();
        public IReadOnlyProperty<float> Angle2 { get; } = new Property<float>();
        public IProperty<string> Message { get; } = new Property<string>();


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
                Message.Value = languageDC.PleaseWait.Value;
            else
                Message.Value = message;
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
                Angle0.ToSettable().Value = Angle0.Value + 3;

                if (Angle0.Value > 360)
                    Angle0.ToSettable().Value = Angle0.Value - 360;

                Angle1.ToSettable().Value = Angle1.Value - 4;

                if (Angle1.Value < 0)
                    Angle1.ToSettable().Value = Angle1.Value + 360;

                Angle2.ToSettable().Value = Angle2.Value + 5;

                if (Angle2.Value > 360)
                    Angle2.ToSettable().Value = Angle2.Value - 360;

                Thread.Sleep(20);
            }
        }
    }
}
