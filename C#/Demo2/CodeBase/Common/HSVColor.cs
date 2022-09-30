namespace Common
{
    public struct HSVColor
    {
        readonly public double Hue;
        readonly public double Saturation;
        readonly public double Value;


        public HSVColor(double hue, double saturation, double value)
        {
            Hue = hue;
            Saturation = saturation;
            Value = value;
        }
    }
}