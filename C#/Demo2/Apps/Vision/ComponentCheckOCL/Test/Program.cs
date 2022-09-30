using System.Diagnostics;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            int color_number = 512;
            int true_color = 256 * 256 * 256;
            int step = true_color / color_number;

            Debug.WriteLine(true_color);

            for (int i = 0; i<true_color;i+=step)
            {
                Debug.WriteLine(i + " r:" + ((i & 0xff0000)>>16) + " g:" + ((i & 0xff00) >> 8) + " b:" + (i & 0xff));
            }

        }
    }
}
