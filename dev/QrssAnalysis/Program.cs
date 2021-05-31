using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace QrssAnalysis
{
    class Program
    {
        static void Main(string[] args)
        {
            var imagePaths = Directory.GetFiles("../../../../sample-images/").Select(x => Path.GetFullPath(x)).Skip(20);

            int count = 0;
            foreach (string imagePath in imagePaths)
            {
                using Bitmap bmp = new(imagePath);

                double[,] data2d = ReadBitmap2D(bmp);
                double[] data1d = FunctionByColumn(data2d, ArrayMean);

                var plt = new ScottPlot.Plot();
                plt.Title(Path.GetFileName(imagePath));
                plt.AddSignal(data1d);
                var hm = plt.AddHeatmapCoordinated(data2d, 0, data2d.GetLength(1), 1000, 2000, ScottPlot.Drawing.Colormap.Grayscale);
                string filename = Path.GetFullPath($"test-{count:000}.bmp");
                plt.SaveFig(filename);

                Console.WriteLine(filename);
                break;
            }
        }

        public static double ArrayStdev(double[] input)
        {
            if (input is null)
                throw new ArgumentNullException(nameof(input));

            if (input.Length == 0)
                throw new ArgumentException("must not be empty");

            double sum = input.Sum();
            double mean = sum / input.Length;
            double sumVariancesSquared = 0;
            for (int i = 0; i < input.Length; i++)
            {
                double pointVariance = Math.Abs(mean - input[i]);
                double pointVarianceSquared = Math.Pow(pointVariance, 2);
                sumVariancesSquared += pointVarianceSquared;
            }
            double meanVarianceSquared = sumVariancesSquared / input.Length;
            double stdev = Math.Sqrt(meanVarianceSquared);

            return stdev;
        }

        public static double ArrayMean(double[] input)
        {
            if (input is null)
                throw new ArgumentNullException(nameof(input));

            if (input.Length == 0)
                throw new ArgumentException("must not be empty");

            return input.Sum() / input.Length;
        }

        public static double ArrayMax(double[] input)
        {
            if (input is null)
                throw new ArgumentNullException(nameof(input));

            if (input.Length == 0)
                throw new ArgumentException("must not be empty");

            return input.Max();
        }

        public static double ArrayMin(double[] input)
        {
            if (input is null)
                throw new ArgumentNullException(nameof(input));

            if (input.Length == 0)
                throw new ArgumentException("must not be empty");

            return input.Min();
        }

        /// <summary>
        /// Collapse a 2D array into a 1D array by applying a function to each column of values
        /// </summary>
        /// <param name="data">2d input data</param>
        /// <param name="function">function to convert a column of values into a single value</param>
        public static double[] FunctionByColumn(double[,] data, Func<double[], double> function)
        {
            int height = data.GetLength(0);
            int width = data.GetLength(1);

            double[] valsByColumn = new double[width];
            for (int columnIndex = 0; columnIndex < width; columnIndex++)
            {
                double[] columnValues = Enumerable.Range(0, height).Select(rowIndex => data[rowIndex, columnIndex]).ToArray();
                valsByColumn[columnIndex] = function(columnValues);
            }
            return valsByColumn;
        }

        /// <summary>
        /// Slice a 2D array into a smaller one containing the defined rows
        /// </summary>
        public static double[,] GetRows(double[,] input, int rowCount, int rowOffset)
        {
            double[,] output = new double[rowCount, input.GetLength(1)];

            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < input.GetLength(1); j++)
                {
                    output[i, j] = input[i + rowOffset, j];
                }
            }
            return output;
        }

        public static double[,] ReadBitmap2D(Bitmap bmp)
        {
            // lock the image and copy all its bytes
            Rectangle rect = new(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, bmp.PixelFormat);
            int byteCount = Math.Abs(bmpData.Stride) * bmp.Height;
            byte[] bytes = new byte[byteCount];
            Marshal.Copy(bmpData.Scan0, bytes, 0, byteCount);
            bmp.UnlockBits(bmpData);

            // copy data from bytes into 2D array
            int bytesPerPixel = Image.GetPixelFormatSize(bmp.PixelFormat) / 8;
            int bytesPerRow = bmpData.Stride;
            double[,] data = new double[bmp.Height, bmp.Width];
            for (int y = 0; y < bmp.Height; y++)
            {
                int rowOffset = bytesPerRow * y;
                for (int x = 0; x < bmp.Width; x++)
                {
                    int pos = rowOffset + x * bytesPerPixel;
                    byte r = bytes[pos];
                    byte g = bytes[pos + 1];
                    byte b = bytes[pos + 2];
                    //byte a = bytes[pos + 3];
                    data[y, x] = r + g + b;
                }
            }
            return data;
        }
    }
}
