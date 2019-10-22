using System;
using System.Drawing;
using System.Windows.Forms;
using ComputerVision.Entities;
using ComputerVision.Logic;

namespace ComputerVision
{
    public partial class MainForm : Form
    {
        private string sSourceFileName = "";
        private FastImage workImage;
        private Bitmap image = null;

        private FastImage initialWorkImage;
        private Bitmap initialImage;

        public MainForm()
        {
            InitializeComponent();
        }

        private void LoadClick(object sender, EventArgs e)
        {
            openFileDialog.ShowDialog();
            sSourceFileName = openFileDialog.FileName;
            panelSource.BackgroundImage = new Bitmap(sSourceFileName);

            image = new Bitmap(sSourceFileName);
            workImage = new FastImage(image);

            initialImage = new Bitmap(sSourceFileName);
            initialWorkImage = new FastImage(initialImage);
        }

        private void GrayScaleClick(object sender, EventArgs e)
        {
            Methods.GrayScaleFastImage(workImage);

            UpdateWorkImage();
        }

        private void NegateClick(object sender, EventArgs e)
        {
            Methods.NegateFastImage(workImage);

            UpdateWorkImage(); ;
        }

        private void TrackBarDelta_ValueChanged(object sender, EventArgs e)
        {
            var deltaValue = trackBarDelta.Value;

            workImage.Lock();
            initialWorkImage.Lock();

            for (var i = 0; i < workImage.Width; i++)
            {
                for (var j = 0; j < workImage.Height; j++)
                {
                    var color = initialWorkImage.GetPixel(i, j);

                    var redNew = GetColorBasedOnDelta(color.R, deltaValue);
                    var greenNew = GetColorBasedOnDelta(color.G, deltaValue);
                    var blueNew = GetColorBasedOnDelta(color.B, deltaValue);

                    color = Color.FromArgb(redNew, greenNew, blueNew);

                    workImage.SetPixel(i, j, color);
                }
            }

            UpdateWorkImage();

            workImage.Unlock();
            initialWorkImage.Unlock();
        }

        private byte GetColorBasedOnDelta(byte colorCode, int delta)
        {
            var sum = colorCode + delta;

            if (sum > 255)
            {
                return 255;
            }

            if (sum < 0)
            {
                return 0;
            }

            return (byte)sum;
        }

        private int GetA(byte min, int delta)
        {
            return min - delta;
        }

        private int GetB(byte max, int delta)
        {
            return max + delta;
        }

        private void TrackBarIntensity_ValueChanged(object sender, EventArgs e)
        {
            panelDestination.BackgroundImage = null;
            panelDestination.BackgroundImage = workImage.GetBitMap();

            workImage.Unlock();
            initialWorkImage.Unlock();

            var minR = initialWorkImage.RedMinimumValue;
            var maxR = initialWorkImage.RedMaximumValue;
            var minG = initialWorkImage.GreenMinimumValue;
            var maxG = initialWorkImage.GreenMaximumValue;
            var minB = initialWorkImage.BlueMinimumValue;
            var maxB = initialWorkImage.BlueMaximumValue;

            var delta = trackBarIntensity.Value;
            var redA = GetA(minR, delta);
            var redB = GetB(maxR, delta);
            var greenA = GetA(minG, delta);
            var greenB = GetB(maxG, delta);
            var blueA = GetA(minB, delta);
            var blueB = GetB(maxB, delta);

            for (var i = 0; i < initialWorkImage.Width; i++)
            {
                for (var j = 0; j < initialWorkImage.Height; j++)
                {
                    var oldRed = initialWorkImage.GetPixel(i, j).R;
                    var newRed = (redB - redA) * (oldRed - minR) / (maxR - minR) + redA;

                    var oldGreen = initialWorkImage.GetPixel(i, j).G;
                    var newGreen = (greenB - greenA) * (oldGreen - minG) / (maxG - minG) + greenA;

                    var oldBlue = initialWorkImage.GetPixel(i, j).B;
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
                    workImage.SetPixel(i, j, newColor);
                }
            }
        }

        private void ButtonEqualization_Click(object sender, EventArgs e)
        {
            panelDestination.BackgroundImage = null;
            panelDestination.BackgroundImage = workImage.GetBitMap();

            var oldGrayScaleHistogram = initialWorkImage.GrayScaleHistogram;
            var newGrayScaleHistogram = new int[256];
            newGrayScaleHistogram[0] = oldGrayScaleHistogram[0];

            for (var i = 1; i < oldGrayScaleHistogram.Length; i++)
            {
                newGrayScaleHistogram[i] = newGrayScaleHistogram[i - 1] + oldGrayScaleHistogram[i];
            }

            var transf = new int[256];
            for (var i = 0; i < transf.Length; i++)
            {
                transf[i] = (newGrayScaleHistogram[i] * 255) / (initialWorkImage.Width * initialWorkImage.Height);
            }

            workImage.Lock();

            for (var i = 0; i < workImage.Width; i++)
            {
                for (var j = 0; j < workImage.Height; j++)
                {
                    var color = initialWorkImage.GetPixel(i, j);
                    var gray = (color.R + color.G + color.B) / 3;
                    var newColor = Color.FromArgb(transf[gray], transf[gray], transf[gray]);

                    workImage.SetPixel(i, j, newColor);
                }
            }

            workImage.Unlock();
        }

        private void UpdateWorkImage()
        {
            panelDestination.BackgroundImage = null;
            panelDestination.BackgroundImage = workImage.GetBitMap();
        }

        //private int GetIntensity(byte min, byte max, int a, int b)
        //{

        //}
    }
}