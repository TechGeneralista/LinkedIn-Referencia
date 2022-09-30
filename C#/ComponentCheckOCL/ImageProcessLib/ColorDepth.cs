using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace ImageProcessLib
{
    public class ColorDepth
    {
        public WriteableBitmap OutputBitmap { get; private set; }

        public int Depth
        {
            get => depth;
            set
            {
                depth = value;
                GenerateValueTable();
            }
        }

        int depth;
        byte[] valueTable;


        private void GenerateValueTable()
        {
            List<byte> list = new List<byte>();
            float step = (float)256 / depth;

            for (float i = 0; i < 255; i += step)
                list.Add((byte)i);

            valueTable = list.ToArray();
        }

        public unsafe WriteableBitmap Change(WriteableBitmap image)
        {
            if (image.IsFrozen)
                image = image.Clone();

            if (OutputBitmap == null || OutputBitmap.PixelWidth != image.PixelWidth || OutputBitmap.PixelHeight != image.PixelHeight)
                OutputBitmap = new WriteableBitmap(image.PixelWidth, image.PixelHeight, 96, 96, image.Format, null);

            if (OutputBitmap.IsFrozen)
                OutputBitmap = OutputBitmap.Clone();

            int stride = image.BackBufferStride;
            int bpp = image.Format.BitsPerPixel / 8;
            byte* is0 = (byte*)image.BackBuffer.ToPointer();
            byte* os0 = (byte*)OutputBitmap.BackBuffer.ToPointer();

            image.Lock();
            OutputBitmap.Lock();

            for (int y = 0; y < image.PixelHeight; y++)
            {
                for (int x = 0; x < image.PixelWidth; x++)
                {
                    byte* ip = is0 + (y * stride) + (x * bpp);
                    byte* op = os0 + (y * stride) + (x * bpp);

                    *(op) = GetColorComponent(*(ip));          //b
                    *(op + 1) = GetColorComponent(*(ip + 1));  //g
                    *(op + 2) = GetColorComponent(*(ip + 2));  //r
                    *(op + 3) = *(ip + 3);                     //a
                }
            }

            image.Unlock();
            //OutputBitmap.AddDirtyRect(new System.Windows.Int32Rect(0, 0, image.PixelWidth, image.PixelHeight));
            OutputBitmap.Unlock();
            OutputBitmap.Freeze();
            return OutputBitmap;
        }

        private byte GetColorComponent(byte val)
        {
            if (val == 0)
                return 0;

            if (val > valueTable[valueTable.Length - 1])
                return 255;

            for (int i = 0; i < valueTable.Length; i += 1)
            {
                if (valueTable[i] <= val && valueTable[i + 1] >= val)
                    return valueTable[i + 1];
            }

            throw new Exception(nameof(GetColorComponent));
        }
    }
}
