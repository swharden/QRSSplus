using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace QrssPlusTests
{
    class ImageTests
    {
        [Test]
        public void Test_Image_Thumbnails()
        {
            foreach (string imageFilePath in System.IO.Directory.GetFiles(SampleData.GRABER_IMAGES_PATH))
            {
                byte[] bytesIn = File.ReadAllBytes(imageFilePath);

                File.WriteAllBytes(
                    path: Path.GetFileName(imageFilePath) + " thumb-skinny.jpg",
                    bytes: QrssPlus.ImageProcessing.GetThumbnailBytes(bytesIn, 50, 500, 50));

                File.WriteAllBytes(
                    path: Path.GetFileName(imageFilePath) + " thumb-auto.jpg",
                    bytes: QrssPlus.ImageProcessing.GetThumbnailBytes(bytesIn, 50, 250));
            }
        }
    }
}
