using System.Runtime.InteropServices;



namespace ImageSourceDevice.UVC.DirectShow.Internals
{
    internal static class Tools
    {
        public static IPin GetPin(IBaseFilter filter, PinDirection dir, int num)
        {
            IPin[] pin = new IPin[1];
            IEnumPins pinsEnum;
            int n;
            PinDirection pinDir;

            if(filter.EnumPins(out pinsEnum) == 0)
            {
                try
                {
                    while(pinsEnum.Next(1, pin, out n) == 0)
                    {
                        pin[0].QueryDirection(out pinDir);

                        if(pinDir == dir)
                        {
                            if(num == 0)
                                return pin[0];
                            num--;
                        }

                        Marshal.ReleaseComObject(pin[0]);
                        pin[0] = null;
                    }
                }

                finally
                {
                    Marshal.ReleaseComObject(pinsEnum);
                }
            }

            return null;
        }



        public static IPin GetInPin(IBaseFilter filter, int num)
        {
            return GetPin(filter, PinDirection.Input, num);
        }



        public static IPin GetOutPin(IBaseFilter filter, int num)
        {
            return GetPin(filter, PinDirection.Output, num);
        }
    }
}
