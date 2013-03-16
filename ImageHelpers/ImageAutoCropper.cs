/* LaTeX Math IDE
Copyright (C) Daniel Jurcau 2013 

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program. If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Image.Helpers
{
    /// <summary>
    /// Auto-cropes an image
    /// </summary>
    public static class ImageAutoCropper
    {
        /// <summary>
        /// Automatically cropes a bitmap 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static BitmapSource Crop(BitmapSource source)
        {
            if (source.PixelWidth == 1 && source.PixelHeight == 1)
            {
                return source;
            }

            int x1, x2, y1, y2;
            PixelColor[,] pixels = source.GetPixels();
            
            //Find x1
            for (x1 = 0; x1 < source.PixelWidth; x1++)
            {
                bool stop = false;
                for (int y = 0; y < source.PixelHeight; y++)
                {
                    if (pixels[x1, y].ColorBGRA != UInt32.MaxValue)
                    {
                        stop = true;
                    }
                }
                if (stop)
                {
                    if (x1 > 0)
                    {
                        x1 -= 1;
                    }
                    break;
                }
            }
            //Find x2
            for (x2 = source.PixelWidth - 1; x2 >= 0 ; x2--)
            {
                bool stop = false;
                for (int y = 0; y < source.PixelHeight; y++)
                {
                    if (pixels[x2, y].ColorBGRA != UInt32.MaxValue)
                    {
                        stop = true;
                    }
                }
                if (stop)
                {
                    if (x2 < source.PixelWidth - 1)
                    {
                        x2 += 1;
                    }
                    break;
                }
            }
            //Find y1
            for (y1 = 0; y1 < source.PixelHeight; y1++)
            {
                bool stop = false;
                for (int x = 0; x < source.PixelWidth; x++)
                {
                    if (pixels[x, y1].ColorBGRA != UInt32.MaxValue)
                    {
                        stop = true;
                    }
                }
                if (stop)
                {
                    if (y1 > 0)
                    {
                        y1 -= 1;
                    }
                    break;
                }
            }
            //Find y2
            for (y2 = source.PixelHeight - 1; y2 >= 0; y2--)
            {
                bool stop = false;
                for (int x = 0; x < source.PixelWidth; x++)
                {
                    if (pixels[x, y2].ColorBGRA != UInt32.MaxValue)
                    {
                        stop = true;
                    }
                }
                if (stop)
                {
                    if (y2 < source.PixelHeight - 1)
                    {
                        y2 += 1;
                    }
                    break;
                }
            }

            Int32Rect rect = new Int32Rect(x1, y1, x2 - x1 + 1, y2 - y1 + 1);
            var result = new CroppedBitmap(source, rect);
            return result;
        }
    }
}
