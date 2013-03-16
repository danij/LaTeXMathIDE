using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.IO;

namespace Image.Helpers
{
    /// <summary>
    /// Provides an extension for the BitmapSource class
    /// http://stackoverflow.com/questions/1176910/finding-specific-pixel-colors-of-a-bitmapimage
    /// </summary>
    public static class BitmapSourceHelper
    {
        #region Public
        /// <summary>
        /// Loads a Bitmap from a byte array
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static BitmapSource Load(byte[] data, bool crop = true)
        {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = new MemoryStream(data);
            bitmap.EndInit();
            var result = bitmap as BitmapSource;
            if (crop)
            {
                result = ImageAutoCropper.Crop(bitmap);
            }            
            return result;
        }
        /// <summary>
        /// Returns the pixels as a 2-dimensional array
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static PixelColor[,] GetPixels(this BitmapSource source)
        {
            if (source.Format != PixelFormats.Bgra32)
            {
                source = new FormatConvertedBitmap(source, 
                    PixelFormats.Bgra32, null, 0);
            }

            int width = source.PixelWidth;
            int height = source.PixelHeight;
            PixelColor[,] result = new PixelColor[width, height];

            CopyPixels(source, result, width * 4, 0);
            return result;
        }
        #endregion

        #region Private
        /// <summary>
        /// Copies the pixels to a 2-dimensional array
        /// </summary>
        /// <param name="source"></param>
        /// <param name="pixels"></param>
        /// <param name="stride"></param>
        /// <param name="offset"></param>
        private static void CopyPixels(BitmapSource source, PixelColor[,] pixels, 
            int stride, int offset)
        {
            var height = source.PixelHeight;
            var width = source.PixelWidth;
            var pixelBytes = new byte[height * width * 4];
            source.CopyPixels(pixelBytes, stride, 0);
            int y0 = offset / width;
            int x0 = offset - width * y0;
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    pixels[x + x0, y + y0] = new PixelColor
                    {
                        Blue = pixelBytes[(y * width + x) * 4 + 0],
                        Green = pixelBytes[(y * width + x) * 4 + 1],
                        Red = pixelBytes[(y * width + x) * 4 + 2],
                        Alpha = pixelBytes[(y * width + x) * 4 + 3],
                    };
        }
        #endregion
    }
    /// <summary>
    /// The color of a pixel, accesible both as an unsigned int32 value and
    /// as individual R G B components
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct PixelColor
    {
        // 32 bit BGRA 
        [FieldOffset(0)]
        public UInt32 ColorBGRA;
        // 8 bit components
        [FieldOffset(0)]
        public byte Blue;
        [FieldOffset(1)]
        public byte Green;
        [FieldOffset(2)]
        public byte Red;
        [FieldOffset(3)]
        public byte Alpha;
    }
}
