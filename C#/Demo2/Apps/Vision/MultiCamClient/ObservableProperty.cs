using System.ComponentModel;
using System.Runtime.CompilerServices;



namespace MultiCamApp
{
    public class ObservableProperty : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetField<T>(T value, ref T field, [CallerMemberName] string propertyName = null)
        {
            field = value;
            FireEvent(propertyName);
            return true;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) => FireEvent(propertyName);
        private void FireEvent(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
