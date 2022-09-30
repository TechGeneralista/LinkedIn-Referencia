using ImageProcessLib.Views.Wait;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;



namespace ImageProcessLib.Components
{
    public class ShapeRotate
    {
        public WriteableBitmap ReferenceImage { get; set; }
        public WriteableBitmap[] Shapes { get; private set; }
        public int[] Angles { get; private set; }


        private double DegToRad(int deg) => (deg * Math.PI) / 180d;

        public Task CreateAsync(int angleTolerance) => Task.Run(() => Create(angleTolerance));

        public unsafe void Create(int angleTolerance)
        {
            if (ReferenceImage is null)
            {
                Shapes = new WriteableBitmap[0];
                Angles = new int[0];
                return;
            }

            if (angleTolerance == 0)
            {
                Shapes = new WriteableBitmap[] { ReferenceImage };
                Angles = new int[] { 0 };
                return;
            }

            GetPixelsPosition(out int[] iX, out int[] iY, out int lenght);

            int cX = (iX.Min() + iX.Max()) / 2;
            int cY = (iY.Min() + iY.Max()) / 2;

            MoveTo(iX, iY, -cX, -cY, lenght);

            List<WriteableBitmap> shapes = new List<WriteableBitmap>();
            List<int> angles = new List<int>();
            object lockObject = new object();

            for (int angle = -angleTolerance; angle <= angleTolerance; angle++)
            //Parallel.For(-angleTolerance, angleTolerance+1, angle=>
            {
                if(angle == 0)
                {
                    shapes.Add(ReferenceImage);
                    angles.Add(0);
                }
                else
                {
                    int[] oX = new int[lenght];
                    int[] oY = new int[lenght];

                    Rotate(iX, iY, oX, oY, angle, lenght);
                    MoveTo(oX, oY, Math.Abs(oX.Min()), Math.Abs(oY.Min()), lenght);

                    WriteableBitmap bitmap = GetBitmap(oX, oY, lenght);

                    lock(lockObject)
                    {
                        shapes.Add(bitmap);
                        angles.Add(angle);
                    }
                }
            }//);

            Shapes = shapes.ToArray();
            Angles = angles.ToArray();
        }

        private unsafe WriteableBitmap GetBitmap(int[] oX, int[] oY, int lenght)
        {
            WriteableBitmap bitmap = new WriteableBitmap(oX.Max()+1, oY.Max()+1, 96, 96, ReferenceImage.Format, null);
            int width = bitmap.PixelWidth;
            int height = bitmap.PixelHeight;
            int stride = bitmap.BackBufferStride;
            int bpp = bitmap.Format.BitsPerPixel / 8;
            byte* scan0 = (byte*)bitmap.BackBuffer.ToPointer();

            bitmap.Lock();

            for (int i = 0; i < lenght; i++)
            //Parallel.For(0, lenght, i =>
            {
                int x = oX[i];
                int y = oY[i];

                byte* ptr = scan0 + (y * stride) + (x * bpp);

                *(ptr) = 255;
                *(ptr+1) = 255;
                *(ptr+2) = 255;
                *(ptr+3) = 255;
            }//);

            for (int y = 0; y < height; y++)
            //Parallel.For(0, height, y =>
            {
                for (int x = 0; x < width; x++)
                {
                    byte* ptr = scan0 + (y * stride) + (x * bpp);

                    if (*ptr != 255)
                        *(ptr + 3) = 255;
                }
            }//);

            bitmap.Unlock();
            bitmap.Freeze();
            return bitmap;
        }

        private void Rotate(int[] iX, int[] iY, int[] oX, int[] oY, int angle, int lenght)
        {
            double rad = DegToRad(angle);

            double s = Math.Sin(rad);
            double c = Math.Cos(rad);

            for (int i = 0; i < lenght; i++)
            //Parallel.For(0, lenght, i =>
            {
                oX[i] = (int)Math.Round((iX[i] * c - iY[i] * s), 0);
                oY[i] = (int)Math.Round((iX[i] * s + iY[i] * c), 0);
            }//);
        }

        private void MoveTo(int[] iX, int[] iY, int cX, int cY, int lenght)
        {
            Parallel.For(0, lenght, i =>
              {
                  iX[i] += cX;
                  iY[i] += cY;
              });
        }

        private unsafe void GetPixelsPosition(out int[] iX, out int[] iY, out int lenght)
        {
            WriteableBitmap referenceImage = ReferenceImage.Clone();
            int rWidth = referenceImage.PixelWidth;
            int rHeight = referenceImage.PixelHeight;
            int rStride = referenceImage.BackBufferStride;
            int rBpp = referenceImage.Format.BitsPerPixel / 8;
            byte* scan0 = (byte*)referenceImage.BackBuffer.ToPointer();
            int lenghtBuff = 0;

            referenceImage.Lock();

            for (int y = 0; y < rHeight; y++)
            //Parallel.For(0, rHeight, y =>
              {
                  for (int x = 0; x < rWidth; x++)
                  {
                      byte* ptr = scan0 + (y * rStride) + (x * rBpp);

                      if (*ptr == 255)
                          Interlocked.Increment(ref lenghtBuff);
                  }
              }//);

            lenght = lenghtBuff;
            int[] iXbuff = new int[lenght];
            int[] iYbuff = new int[lenght];
            int index = 0;

            for (int y = 0; y < rHeight; y++)
            //Parallel.For(0, rHeight, y =>
            {
                for (int x = 0; x < rWidth; x++)
                {
                    byte* ptr = scan0 + (y * rStride) + (x * rBpp);

                    if (*ptr == 255)
                    {
                        iXbuff[index] = x;
                        iYbuff[index] = y;
                        Interlocked.Increment(ref index);
                    }
                }
            }//);

            referenceImage.Unlock();

            iX = iXbuff;
            iY = iYbuff;
        }
    }
}
