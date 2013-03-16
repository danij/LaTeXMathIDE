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
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace Image.Helpers
{
    /// <summary>
    /// Adds transparency to an image
    /// </summary>
    public static class ImageTransparency
    {
        /// <summary>
        /// Adds transparency to a Bitmapsource
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static BitmapSource AddTransparencyToBlackAndWhiteImage(BitmapSource source)
        {
            uint[] pixels = new uint[source.PixelWidth * source.PixelHeight];
            source.CopyPixels(pixels, source.PixelWidth * 4, 0);

            uint r, g, b, a;
            for (int i = 0; i < pixels.Length; i++)
            {
                var item = pixels[i];
                //ABGR
                a = item >> 24 & 0xFF;
                b = item >> 16 & 0xFF;
                g = item >> 8 & 0xFF;
                r = item & 0xFF;

                uint gray = (r + g + b) / 3;
                a = 255 - gray;
                r = g = b = 0;
                pixels[i] = a << 24 | b << 16 | g << 8 | r;
            }

            return BitmapSource.Create(source.PixelWidth, source.PixelHeight,
                source.DpiX, source.DpiY, PixelFormats.Bgra32, source.Palette,
                pixels, source.PixelWidth * 4);
        }
    }
}
