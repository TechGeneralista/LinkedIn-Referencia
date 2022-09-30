using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace Common
{
    public class DCBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        protected void SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            field = value;
            OnPropertyChanged(propertyName);
        }

        protected void FieldSetted([CallerMemberName] string propertyName = null)
            => OnPropertyChanged(propertyName);
    }
}
