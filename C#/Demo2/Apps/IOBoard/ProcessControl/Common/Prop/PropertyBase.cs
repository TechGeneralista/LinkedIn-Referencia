using System.ComponentModel;
using System.Runtime.CompilerServices;



namespace Common.Prop
{
    public class PropertyBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetField<T>(T value, ref T field, [CallerMemberName] string propertyName = null)
        {
            field = value;
            FireEvent(propertyName);
            return true;
        }

        private void FireEvent(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
