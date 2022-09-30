using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace Common.ObservableProperty
{
    public class ObservableValueBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;


        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
