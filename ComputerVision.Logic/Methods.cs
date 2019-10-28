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

        public static void ChangeIntensityForFastImage(FastImage fastImage, FastImage originalFastImage, int intensity)
        {
            fastImage.Lock();
            originalFastImage.Lock();

            var minR = originalFastImage.RedMinimumValue;
            var maxR = originalFastImage.RedMaximumValue;
            var minG = originalFastImage.GreenMinimumValue;
            var maxG = originalFastImage.GreenMaximumValue;
            var minB = originalFastImage.BlueMinimumValue;
            var maxB = originalFastImage.BlueMaximumValue;

            var delta = intensity;
            var redA = GetA(minR, delta);
            var redB = GetB(maxR, delta);
            var greenA = GetA(minG, delta);
            var greenB = GetB(maxG, delta);
            var blueA = GetA(minB, delta);
            var blueB = GetB(maxB, delta);

            for (var i = 0; i < originalFastImage.Width; i++)
            {
                for (var j = 0; j < originalFastImage.Height; j++)
                {
                    var oldRed = originalFastImage.GetPixel(i, j).R;
                    var newRed = (redB - redA) * (oldRed - minR) / (maxR - minR) + redA;

                    var oldGreen = originalFastImage.GetPixel(i, j).G;
                    var newGreen = (greenB - greenA) * (oldGreen - minG) / (maxG - minG) + greenA;

                    var oldBlue = originalFastImage.GetPixel(i, j).B;
                    var newBlue = (blueB - blueA) * (oldBlue - minB) / (maxB - minB) + blueA;

                    if (newRed > 255)
                    {
                        newRed = 255;
                    }
                    else if (newRed < 0)
                    {
                        newRed = 0;
                    }

                    if (newGreen > 255)
                    {
                        newGreen = 255;
                    }
                    else if (newGreen < 0)
                    {
                        newGreen = 0;
                    }

                    if (newBlue > 255)
                    {
                        newBlue = 255;
                    }
                    else if (newBlue < 0)
                    {
                        newBlue = 0;
                    }

                    var newColor = Color.FromArgb(newRed, newGreen, newBlue);
                    fastImage.SetPixel(i, j, newColor);
                }
            }

            fastImage.Unlock();
            originalFastImage.Unlock();
        }

        private static int GetA(byte min, int delta)
        {
            return min - delta;
        }

        private static int GetB(byte max, int delta)
        {
            return max + delta;
        }

        public static void ApplyEqualization(FastImage fastImage, FastImage originalFastImage)
        {
            var oldGrayScaleHistogram = originalFastImage.GrayScaleHistogram;
            var newGrayScaleHistogram = new int[256];
            newGrayScaleHistogram[0] = oldGrayScaleHistogram[0];

            for (var i = 1; i < oldGrayScaleHistogram.Length; i++)
            {
                newGrayScaleHistogram[i] = newGrayScaleHistogram[i - 1] + oldGrayScaleHistogram[i];
            }

            var transf = new int[256];
            for (var i = 0; i < transf.Length; i++)
            {
                transf[i] = (newGrayScaleHistogram[i] * 255) / (originalFastImage.Width * originalFastImage.Height);
            }

            originalFastImage.Lock();
            fastImage.Lock();

            for (var i = 0; i < fastImage.Width; i++)
            {
                for (var j = 0; j < fastImage.Height; j++)
                {
                    var color = originalFastImage.GetPixel(i, j);
                    var gray = (color.R + color.G + color.B) / 3;
                    var newColor = Color.FromArgb(transf[gray], transf[gray], transf[gray]);

                    fastImage.SetPixel(i, j, newColor);
                }
            }

            originalFastImage.Unlock();
            fastImage.Unlock();
        }

        public static void LowPassFiler(FastImage fastImage, FastImage originalFastImage)
        {
            fastImage.Lock();
            originalFastImage.Lock();



            fastImage.Unlock();
            fastImage.Unlock();
        }
    }
}
