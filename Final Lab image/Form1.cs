using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using openCV;


namespace ImageProcessing
{
    public partial class Form1 : Form
    {


        bool imageSet;
        bool nchartset;
        System.Windows.Forms.DataVisualization.Charting.Chart chartCopy;
        System.Windows.Forms.DataVisualization.Charting.Chart saved_chart;

        public Form1()
        {
            InitializeComponent();

            imageSet = false;

            chartCopy = chart1;
            nchartset = false;
            


        }
        IplImage image1;
        IplImage img;
        Bitmap bmp;


        private bool ImageNotSet()
        {
            if (!imageSet) {
                MessageBox.Show("You have to set the image first");
            }

            return !imageSet;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = " ";
            openFileDialog1.Filter = "JPEG|*JPG|Bitmap|*.bmp|All|*.*";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    image1 = cvlib.CvLoadImage(openFileDialog1.FileName, cvlib.CV_LOAD_IMAGE_COLOR);
                    CvSize size = new CvSize(pictureBox1.Width, pictureBox1.Height);
                    IplImage resized_image = cvlib.CvCreateImage(size, image1.depth, image1.nChannels);
                    cvlib.CvResize(ref image1, ref resized_image, cvlib.CV_INTER_LINEAR);
                    pictureBox1.BackgroundImage = (Image)resized_image;

                    imageSet = true;

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            imageSet = true;
        }

        private void redToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ImageNotSet())
                return;

            img = cvlib.CvCreateImage(new CvSize(image1.width, image1.height), image1.depth, image1.nChannels);
            int srcAdd = image1.imageData.ToInt32();
            int dstAdd = img.imageData.ToInt32();

            unsafe
            {
                int srcIndex, dstIndex;
                for (int r = 0; r < img.height; r++)
                {
                    for (int c = 0; c < img.width; c++)
                    {
                        srcIndex = dstIndex = (img.width * r * img.nChannels) + (c * img.nChannels);
                        *(byte*)(dstAdd + dstIndex + 0) = 0;       // 0 =Blue
                        *(byte*)(dstAdd + dstIndex + 1) = 0;        // 1 = green
                        *(byte*)(dstAdd + dstIndex + 2) = *(byte*)(srcAdd + srcIndex + 2); // 2 = red
                    }
                }

            }
            DisplayImageInPictureBox();
        }

        private void greenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ImageNotSet())
                return;

            img = cvlib.CvCreateImage(new CvSize(image1.width, image1.height), image1.depth, image1.nChannels);
            int srcAdd = image1.imageData.ToInt32();
            int dstAdd = img.imageData.ToInt32();

            unsafe
            {
                int srcIndex, dstIndex;
                for (int r = 0; r < img.height; r++)
                {
                    for (int c = 0; c < img.width; c++)
                    {
                        srcIndex = dstIndex = (img.width * r * img.nChannels) + (c * img.nChannels);
                        *(byte*)(dstAdd + dstIndex + 0) = 0;       // 0 =Blue
                        *(byte*)(dstAdd + dstIndex + 1) = *(byte*)(srcAdd + srcIndex + 1);        // 1 = green
                        *(byte*)(dstAdd + dstIndex + 2) = 0; // 2 = red
                    }
                }

            }
            DisplayImageInPictureBox();
        }

        private void blueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ImageNotSet())
                return;

            img = cvlib.CvCreateImage(new CvSize(image1.width, image1.height), image1.depth, image1.nChannels);
            int srcAdd = image1.imageData.ToInt32();
            int dstAdd = img.imageData.ToInt32();

            unsafe
            {
                int srcIndex, dstIndex;
                for (int r = 0; r < img.height; r++)
                {
                    for (int c = 0; c < img.width; c++)
                    {
                        srcIndex = dstIndex = (img.width * r * img.nChannels) + (c * img.nChannels);
                        *(byte*)(dstAdd + dstIndex + 0) = *(byte*)(srcAdd + srcIndex + 0);       // 0 =Blue
                        *(byte*)(dstAdd + dstIndex + 1) = 0;        // 1 = green
                        *(byte*)(dstAdd + dstIndex + 2) = 0; // 2 = red
                    }
                }

            }
            DisplayImageInPictureBox();
        }



        private void DisplayImageInPictureBox()
        {
            CvSize size = new CvSize(pictureBox2.Width, pictureBox2.Height);
            IplImage resized_image = cvlib.CvCreateImage(size, img.depth, img.nChannels);
            cvlib.CvResize(ref img, ref resized_image, cvlib.CV_INTER_LINEAR);
            pictureBox2.BackgroundImage = (Image)resized_image;
        }

        private void histogramToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (ImageNotSet())
                return;

            Bitmap bmpImg = (Bitmap)image1;
            int width = bmpImg.Width;
            int height = bmpImg.Height;
            int[] channel_Red = new int[256];
            int[] channel_Green = new int[256];
            int[] channel_Blue = new int[256];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Color pixelColor = bmpImg.GetPixel(i, j);
                    channel_Red[pixelColor.R]++;
                    channel_Green[pixelColor.G]++;
                    channel_Blue[pixelColor.B]++;
                }
            }


            for (int i = 0; i < 256; i++)
            {
                chart1.Series["Red"].Points.AddY(channel_Red[i]);
                chart1.Series["Green"].Points.AddY(channel_Green[i]);
                chart1.Series["Blue"].Points.AddY(channel_Blue[i]);
            }

        }

        private void equalizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ImageNotSet())
                return;

            try
            {
                chart1.Series.Clear();
                chart1 = chartCopy;

                Bitmap bmpImg = (Bitmap)image1;
                int width = bmpImg.Width;
                int height = bmpImg.Height;

                // Initialize histograms for the RGB channels
                int[] channel_Red = new int[256];
                int[] channel_Green = new int[256];
                int[] channel_Blue = new int[256];

                // Calculate the histogram (N(i)) for each color channel
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        Color pixelColor = bmpImg.GetPixel(i, j);
                        channel_Red[pixelColor.R]++;
                        channel_Green[pixelColor.G]++;
                        channel_Blue[pixelColor.B]++;
                    }
                }

                // Create and configure the Red histogram series
                Series redSeries = new Series("Red Histogram")
                {
                    ChartType = SeriesChartType.Column
                };
                for (int i = 0; i < 256; i++)
                {
                    redSeries.Points.AddY(channel_Red[i]);
                }
                chart1.Series.Add(redSeries);

                // Create and configure the Green histogram series
                Series greenSeries = new Series("Green Histogram")
                {
                    ChartType = SeriesChartType.Column
                };
                for (int i = 0; i < 256; i++)
                {
                    greenSeries.Points.AddY(channel_Green[i]);
                }
                chart1.Series.Add(greenSeries);

                // Create and configure the Blue histogram series
                Series blueSeries = new Series("Blue Histogram")
                {
                    ChartType = SeriesChartType.Column
                };
                for (int i = 0; i < 256; i++)
                {
                    blueSeries.Points.AddY(channel_Blue[i]);
                }
                chart1.Series.Add(blueSeries);

                // Customize chart appearance
                chart1.ChartAreas[0].AxisX.Title = "Intensity";
                chart1.ChartAreas[0].AxisY.Title = "Frequency";
                chart1.Legends.Clear();
                chart1.Invalidate();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error during histogram plotting: " + ex.Message);
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void gaussianBlurToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ImageNotSet())
                return;

            try
            {
                img = cvlib.CvCreateImage(new CvSize(image1.width, image1.height), image1.depth, image1.nChannels);

                cvlib.CvSmooth(ref image1, ref img, cvlib.CV_GAUSSIAN, 7, 7, 0, 0);

                DisplayImageInPictureBox();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void grayScaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ImageNotSet())
                return;

            try
            {
                img = cvlib.CvCreateImage(new CvSize(image1.width, image1.height), image1.depth, image1.nChannels);

                int srcAdd = image1.imageData.ToInt32();
                int dstAdd = img.imageData.ToInt32();

                unsafe
                {
                    int srcIndex, dstIndex;
                    for (int r = 0; r < img.height; r++) // Iterate over rows
                    {
                        for (int c = 0; c < img.width; c++) // Iterate over columns
                        {
                            srcIndex = dstIndex = (img.width * r * img.nChannels) + (c * img.nChannels);

                            byte b = *(byte*)(srcAdd + srcIndex + 0); // Blue
                            byte g = *(byte*)(srcAdd + srcIndex + 1); // Green
                            byte red = *(byte*)(srcAdd + srcIndex + 2); // Red

                            // Calculate grayscale value
                            int avg = (red + g + b) / 3;

                            // Set the grayscale value to all channels
                            *(byte*)(dstAdd + dstIndex + 0) = (byte)avg; // Blue
                            *(byte*)(dstAdd + dstIndex + 1) = (byte)avg; // Green
                            *(byte*)(dstAdd + dstIndex + 2) = (byte)avg; // Red
                        }
                    }
                }

                DisplayImageInPictureBox();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void cannyEdgeDetectionToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (ImageNotSet())
                return;

            try
            {
                img = cvlib.CvCreateImage(new CvSize(image1.width, image1.height), image1.depth, image1.nChannels);

                int srcAdd = image1.imageData.ToInt32();
                int dstAdd = img.imageData.ToInt32();

                unsafe
                {
                    int srcIndex, dstIndex;
                    for (int r = 1; r < img.height - 1; r++)
                    {
                        for (int c = 1; c < img.width - 1; c++)
                        {
                            srcIndex = dstIndex = (img.width * r * img.nChannels) + (c * img.nChannels);

                            // Sobel Kernel for detecting edges in the x and y directions
                            int gx = 0, gy = 0;

                            // Sobel X kernel
                            gx += -1 * *(byte*)(srcAdd + (img.width * (r - 1) * img.nChannels) + ((c - 1) * img.nChannels)); // Top-left
                            gx += 0 * *(byte*)(srcAdd + (img.width * (r - 1) * img.nChannels) + (c * img.nChannels));      // Top-center
                            gx += 1 * *(byte*)(srcAdd + (img.width * (r - 1) * img.nChannels) + ((c + 1) * img.nChannels)); // Top-right
                            gx += -2 * *(byte*)(srcAdd + (img.width * r * img.nChannels) + ((c - 1) * img.nChannels));       // Mid-left
                            gx += 0 * *(byte*)(srcAdd + (img.width * r * img.nChannels) + (c * img.nChannels));            // Mid-center
                            gx += 2 * *(byte*)(srcAdd + (img.width * r * img.nChannels) + ((c + 1) * img.nChannels));       // Mid-right
                            gx += -1 * *(byte*)(srcAdd + (img.width * (r + 1) * img.nChannels) + ((c - 1) * img.nChannels)); // Bottom-left
                            gx += 0 * *(byte*)(srcAdd + (img.width * (r + 1) * img.nChannels) + (c * img.nChannels));      // Bottom-center
                            gx += 1 * *(byte*)(srcAdd + (img.width * (r + 1) * img.nChannels) + ((c + 1) * img.nChannels)); // Bottom-right

                            // Sobel Y kernel
                            gy += -1 * *(byte*)(srcAdd + (img.width * (r - 1) * img.nChannels) + ((c - 1) * img.nChannels)); // Top-left
                            gy += -2 * *(byte*)(srcAdd + (img.width * (r - 1) * img.nChannels) + (c * img.nChannels));       // Top-center
                            gy += -1 * *(byte*)(srcAdd + (img.width * (r - 1) * img.nChannels) + ((c + 1) * img.nChannels)); // Top-right
                            gy += 1 * *(byte*)(srcAdd + (img.width * (r + 1) * img.nChannels) + ((c - 1) * img.nChannels)); // Bottom-left
                            gy += 2 * *(byte*)(srcAdd + (img.width * (r + 1) * img.nChannels) + (c * img.nChannels));       // Bottom-center
                            gy += 1 * *(byte*)(srcAdd + (img.width * (r + 1) * img.nChannels) + ((c + 1) * img.nChannels)); // Bottom-right

                            // Compute the gradient magnitude (edge strength)
                            int magnitude = (int)Math.Sqrt(gx * gx + gy * gy);
                            magnitude = Math.Min(255, magnitude); // Ensure the value is in the 0-255 range

                            // Set the edge value to all channels (grayscale)
                            *(byte*)(dstAdd + dstIndex + 0) = (byte)magnitude; // Blue
                            *(byte*)(dstAdd + dstIndex + 1) = (byte)magnitude; // Green
                            *(byte*)(dstAdd + dstIndex + 2) = (byte)magnitude; // Red
                        }
                    }
                }

                // Display the resulting edge-detected image
                DisplayImageInPictureBox();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
    }
}
