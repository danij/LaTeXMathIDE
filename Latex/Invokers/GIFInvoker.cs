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

namespace Latex.Invokers
{
    /// <summary>
    /// Invokes LaTeX to output a GIF file from an intermediate DVI file
    /// </summary>
    public class GIFInvoker : DVIInvoker
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public GIFInvoker()
        {
        }
        /// <summary>
        /// Gets or sets the resolution for the output file
        /// </summary>
        public int Resolution { get; set; }
        /// <summary>
        /// Checks if the necessary tools are installed
        /// </summary>
        /// <param name="folder">LaTeX folder</param>
        /// <returns></returns>
        public override bool CheckTools()
        {
            return base.CheckTools() & QuickCheckToolFiles("dvipng.exe");
        }
        /// <summary>
        /// Invokes LaTeX
        /// </summary>
        /// <param name="inputFileName">The input file name</param>
        /// <returns>The output file name</returns>
        public override InvokeResult Invoke(string inputFileName)
        {
            var extension = ".gif";
            var filePrefix = Path.GetDirectoryName(inputFileName) + "\\" +
                Path.GetFileNameWithoutExtension(inputFileName);
            var resultFileName = filePrefix + extension;
            var result = new InvokeResult();

            var dviFile = filePrefix + ".dvi"; 
            var process = PrepareProcess("dvipng",
                String.Format("--gif -D {1} \"{0}\"", dviFile, Resolution));

            try
            {
                File.WriteAllBytes(dviFile, base.Invoke(inputFileName).Bytes);

                process.Start();
                process.WaitForExit();

                var outputFile = filePrefix + "1" + extension;

                if (File.Exists(outputFile))
                {
                    result.Bytes = File.ReadAllBytes(outputFile);
                    Clean(outputFile);
                }
            }
            catch (Exception ex)
            {
                result.Exception = ex;
            }
            finally
            {
                Clean(resultFileName);
            }

            return result;
        }
    }
}
