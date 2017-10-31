using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BrushEditor
{
    public class SimpleBitmap
    {
        private readonly int _height;
        private readonly int _width;

        public SimpleBitmap(int width, int height)
        {
            _width = width;
            _height = height;

            BaseBitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgr32, null);
        }

        public WriteableBitmap BaseBitmap { get; private set; }

        public uint GetPixel(int x, int y)
        {
            unsafe
            {
                return *(uint*) (((byte*) BaseBitmap.BackBuffer.ToPointer()) + BaseBitmap.BackBufferStride*y + x*4);
            }
        }

        public void SetPixel(int x, int y, uint c)
        {
            BaseBitmap.Lock();
            SetPixelUnlocked(x, y, c);
            BaseBitmap.AddDirtyRect(new Int32Rect(x, y, 1, 1));
            BaseBitmap.Unlock();
        }

        private void SetPixelUnlocked(int x, int y, uint c)
        {
            unsafe
            {
                *(uint*) (((byte*) BaseBitmap.BackBuffer.ToPointer()) + BaseBitmap.BackBufferStride*y + x*4) = c;
            }
        }

        public Color GetColor(int x, int y)
        {
            byte[] p = BitConverter.GetBytes(GetPixel(x, y));
            return Color.FromArgb(p[3], p[2], p[1], p[0]);
        }

        public void SetColor(int x, int y, Color c, bool perfromLock)
        {
            if (perfromLock)
                SetPixel(x, y, UIntFromColor(c));
            else
                SetPixelUnlocked(x, y, UIntFromColor(c));
        }

        private uint UIntFromColor(Color color)
        {
            return (uint) color.A << 24 | ((uint) color.R << 16) | ((uint) color.G << 8) | color.B;
        }

        public void SetPixelsFromArray(Color[,] colors)
        {
            BaseBitmap.Lock();

            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    uint c = UIntFromColor(colors[x, y]);

                    SetPixelUnlocked(x, y, c);
                }
            }

            BaseBitmap.AddDirtyRect(new Int32Rect(0, 0, _width, _height));
            BaseBitmap.Unlock();
        }
    }
}