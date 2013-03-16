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
using System.Diagnostics;
using Latex.Exceptions;

namespace Latex.Invokers
{
    /// <summary>
    /// Invokes LaTeX to output a PNG file
    /// </summary>
    public class DVIInvoker : AbstractInvoker
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public DVIInvoker()
        {            
        }
        /// <summary>
        /// Checks if the necessary tools are installed
        /// </summary>
        /// <param name="folder">LaTeX folder</param>
        /// <returns></returns>
        public override bool CheckTools()
        {
            return QuickCheckToolFiles("latex.exe");
        }
        /// <summary>
        /// Invokes LaTeX
        /// </summary>
        /// <param name="inputFileName">The input file name</param>
        /// <returns></returns>
        public override InvokeResult Invoke(string inputFileName)
        {
            if (!CheckTools())
            {
                throw new ToolsNotFoundException();
            }
            var extension = ".dvi";
            var filePrefix = Path.GetDirectoryName(inputFileName) + "\\" +
                Path.GetFileNameWithoutExtension(inputFileName);
            var resultFileName = filePrefix + extension;
            var result = new InvokeResult();

            var process = PrepareProcess("latex", String.Format(
                "-halt-on-error -output-format=dvi \"{0}\"", 
                inputFileName));

            process.Start();
            var reader = process.StandardOutput;
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (line.StartsWith("! "))
                {
                    var message = line + reader.ReadLine();
                    result.Exception = new CompilationFailedException(message);                    
                }
            }
            process.WaitForExit();

            if (File.Exists(resultFileName))
            {
                result.Bytes = File.ReadAllBytes(resultFileName);
                Clean(resultFileName);
            }

            if (result.Exception != null)
            {
                throw result.Exception;
            }

            return result;
        }
    }
}
