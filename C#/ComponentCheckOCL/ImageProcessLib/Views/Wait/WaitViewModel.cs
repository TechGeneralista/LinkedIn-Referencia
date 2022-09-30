using CommonLib.Components;

namespace ImageProcessLib.Views.Wait
{
    public class WaitViewModel : ObservableProperty
    {
        public int Minimum 
        {
            get => minimum;
            set => SetField(value, ref minimum);
        }

        public int Value
        {
            get => val;
            set => SetField(value, ref val);
        }

        public int Maximum
        {
            get => maximum;
            set => SetField(value, ref maximum);
        }


        int minimum, val, maximum;
    }
}
