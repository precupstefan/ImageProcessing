using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace ComputerVision.Entities
{
    public class FastImage
    {
        private bool locked;
        public bool Locked
        {
            get => locked;
            set
            {
                locked = value;

                if (!locked)
                {
                    UpdateColorMinimumAndMaximumValues();
                }
            }
        }

        public int Height { get; set; }
        public int Width { get; set; }

        public byte RedMinimumValue { get; private set; }
        public byte RedMaximumValue { get; private set; }
        public byte GreenMinimumValue { get; private set; }
        public byte GreenMaximumValue { get; private set; }
        public byte BlueMinimumValue { get; private set; }
        public byte BlueMaximumValue { get; private set; }

        private Bitmap image = null;
        private Rectangle rectangle;
        private BitmapData bitmapData = null;
        private Color color;
        private Point size;
        private readonly int currentBitmapWidth = 0;

        public FastImage(Bitmap bitmap)
        {
            image = bitmap;
            Width = image.Width;
            Height = image.Height;
            size = new Point(image.Size);
            currentBitmapWidth = size.X;

            Locked = false;

            UpdateColorMinimumAndMaximumValues();
        }

        public void Lock(bool update = true)
        {
            rectangle = new Rectangle(0, 0, image.Width, image.Height);
            bitmapData = image.LockBits(rectangle, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            if (update)
            {
                Locked = true;
            }
            else
            {
                locked = true;
            }
        }

        public void Unlock(bool update = true)
        {
            image.UnlockBits(bitmapData);

            if (update)
            {
                Locked = false;
            }
            else
            {
                locked = false;
            }
        }

        public Color GetPixel(int col, int row)
        {
            if (!Locked)
            {
                throw new UnauthorizedAccessException();
            }

            unsafe
            {
                PixelData* pBase = (PixelData*)bitmapData.Scan0;
                PixelData* pPixel = pBase + row * currentBitmapWidth + col;
                color = Color.FromArgb(pPixel->Red, pPixel->Green, pPixel->Blue);
            }

            return color;
        }

        public void SetPixel(int col, int row, Color c)
        {
            if (!Locked)
            {
                throw new UnauthorizedAccessException();
            }

            unsafe
            {
                PixelData* pBase = (PixelData*)bitmapData.Scan0;
                PixelData* pPixel = pBase + row * currentBitmapWidth + col;
                pPixel->Red = c.R;
                pPixel->Green = c.G;
                pPixel->Blue = c.B;
            }
        }

        public Bitmap GetBitMap()
        {
            return image;
        }

        public int[] GrayScaleHistogram
        {
            get
            {
                var histogram = new int[256];

                this.Lock();

                for (var i = 0; i < this.Width; i++)
                {
                    for (var j = 0; j < this.Height; j++)
                    {
                        var color = this.GetPixel(i, j);
                        var gray = (color.R + color.G + color.B) / 3;

                        histogram[gray]++;
                    }
                }

                this.Unlock();

                return histogram;
            }
        }

        private void UpdateColorMinimumAndMaximumValues()
        {
            byte minR = 255;
            byte maxR = 0;

            byte minG = 255;
            byte maxG = 0;

            byte minB = 255;
            byte maxB = 0;

            this.Lock(false);

            Color color;

            // Get min and max values for R G B
            for (int i = 0; i < this.Width; i++)
            {
                for (int j = 0; j < this.Height; j++)
                {
                    color = this.GetPixel(i, j);

                    // Update red min and max
                    if (color.R < minR)
                    {
                        minR = color.R;
                    }

                    if (color.R > maxR)
                    {
                        maxR = color.R;
                    }

                    // Update green min and max
                    if (color.G < minG)
                    {
                        minG = color.G;
                    }

                    if (color.G > maxG)
                    {
                        maxG = color.G;
                    }

                    // Update blue min and max
                    if (color.B < minB)
                    {
                        minB = color.B;
                    }

                    if (color.B > maxB)
                    {
                        maxB = color.B;
                    }
                }
            }

            this.Unlock(false);

            RedMinimumValue = minR;
            RedMaximumValue = maxR;
            GreenMinimumValue = minG;
            GreenMaximumValue = maxG;
            BlueMinimumValue = minB;
            BlueMaximumValue = maxB;
        }
    }
}
