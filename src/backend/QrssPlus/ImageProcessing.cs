using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace QrssPlus
{
    public static class ImageProcessing
    {
        public static byte[] GetThumbnailSkinny(byte[] bytes) => GetThumbnailBytes(bytes, quality: 50, height: 500, width: 25);

        public static byte[] GetThumbnailAuto(byte[] bytes) => GetThumbnailBytes(bytes, quality: 85, height: 100, width: 150);

        public static byte[] GetThumbnailBytes(byte[] bytes, int quality, int height, int width = -1)
        {
            using MemoryStream msIn = new MemoryStream(bytes);
            Image originalImage = Bitmap.FromStream(msIn);

            if (width <= 0)
                width = (int)(height * ((double)originalImage.Width / originalImage.Height));

            Image thumbnailImage = Resize(originalImage, width, height);

            using MemoryStream msOut = new MemoryStream();
            
            EncoderParameters encoderParams;
            try {
                encoderParams = new EncoderParameters(1);
            } catch (Exception e) {
                throw new InvalidOperationException("EncoderParameters construction fail", e);
            }
            
            try {
                encoderParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
            } catch (Exception e) {
                throw new InvalidOperationException("EncoderParameter construction fail", e);
            }
                      
            ImageCodecInfo myImageCodecInfo;  
            try {
                myImageCodecInfo = ImageCodecInfo.GetImageEncoders().Where(x => x.MimeType == "image/jpeg").First();
            } catch (Exception e) {
                throw new InvalidOperationException("ImageCodecInfo fail", e);
            }
                        
            try {
                thumbnailImage.Save(msOut, myImageCodecInfo, encoderParams);
            } catch (Exception e) {
                throw new InvalidOperationException("thumbnail save fail", e);
            }
            
            return msOut.ToArray();
        }

        private static Image Resize(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using var graphics = Graphics.FromImage(destImage);
            graphics.CompositingMode = CompositingMode.SourceCopy;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            using var wrapMode = new ImageAttributes();
            wrapMode.SetWrapMode(WrapMode.TileFlipXY);
            graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);

            return destImage;
        }
    }
}
