using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;



namespace CommonLib
{
    public static class Extensions
    {
        public static void RefreshDataContext(this FrameworkElement frameworkElement)
        {
            object dataContext = frameworkElement.DataContext;
            frameworkElement.DataContext = null;
            frameworkElement.DataContext = dataContext;
        }

        public static bool IsNull(this object obj) => obj is null;
        public static bool IsNotNull(this object obj) => !obj.IsNull();

        public static bool IsNullOrEmpty(this string str) => str is null || str == "";

        public static string IfIsEmptyGenerateNewGUID(this string str)
        {
            if (str.IsNullOrEmpty())
                return Guid.NewGuid().ToString().ToUpper().Replace("-", string.Empty);

            return str;
        }

        public static int GetTrueValuesCount(this bool[,] array)
        {
            int width = array.GetLength(0);
            int height = array.GetLength(1);
            int counter = 0;

            Parallel.For(0, height, y =>
            {
                for (int x = 0; x < width; x++)
                {
                    if (array[x, y])
                        Interlocked.Increment(ref counter);
                }
            });

            return counter;
        }
    }
}
