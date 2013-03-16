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
using System.IO;

namespace Latex.Helpers
{
    /// <summary>
    /// Generates a random file name
    /// </summary>
    public static class RandomFileNameGenerator
    {
        #region Public
        /// <summary>
        /// Generates a random file name
        /// </summary>
        /// <param name="extension"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string Generate(string extension = ".tmp", int length = 10)
        {
            var allowed = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var builder = new StringBuilder();

            if (length < 1)
            {
                throw new ArgumentException("length must be greater than 0");
            }
            
            for (int i = 0; i < length; i++)
            {
                builder.Append(allowed[random.Next(allowed.Length)]);
            }

            builder.Append(extension);

            return builder.ToString();
        }
        /// <summary>
        /// Generates a random file name appended to the temporary folder
        /// </summary>
        /// <param name="extension"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GenerateTemp(string extension = ".tmp", int length = 10)
        {
            return Path.GetTempPath() + "\\" + Generate(extension, length);
        }
        #endregion

        #region Private
        /// <summary>
        /// Random number generator
        /// </summary>
        private static Random random = new Random();
        #endregion
    }
}
