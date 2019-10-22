using System.Drawing;
using ComputerVision.Entities;

namespace ComputerVision.Logic
{
    public static class Methods
    {
        public static void GrayScaleFastImage(FastImage fastImage)
        {
            fastImage.Lock();

            for (var i = 0; i < fastImage.Width; i++)
            {
                for (var j = 0; j < fastImage.Height; j++)
                {
                    var color = fastImage.GetPixel(i, j);
                    var average = (byte)((color.R + color.G + color.B) / 3);

                    color = Color.FromArgb(average, average, average);

                    fastImage.SetPixel(i, j, color);
                }
            }

            fastImage.Unlock();
        }

        public static void NegateFastImage(FastImage fastImage)
        {
            fastImage.Lock();

            for (var i = 0; i < fastImage.Width; i++)
            {
                for (var j = 0; j < fastImage.Height; j++)
                {
                    var color = fastImage.GetPixel(i, j);

                    var newRed = (byte)(255 - color.R);
                    var newGreen = (byte)(255 - color.G);
                    var newBlue = (byte)(255 - color.B);

                    color = Color.FromArgb(newRed, newGreen, newBlue);

                    fastImage.SetPixel(i, j, color);
                }
            }

            fastImage.Unlock();
        }
    }
}
