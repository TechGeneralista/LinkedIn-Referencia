using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;



namespace BalanceLib
{
    public class ObservableProperty : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void SetField<T>(T value, ref T field, [CallerMemberName] string propertyName = null)
        {
            field = value;
            FireEvent(propertyName);
        }

        protected bool SetFieldWithCompare<T>(T value, ref T field, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            FireEvent(propertyName);
            return true;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) => FireEvent(propertyName);
        private void FireEvent(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
