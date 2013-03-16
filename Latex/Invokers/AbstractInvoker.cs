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
using Latex.Exceptions;
using System.IO;
using System.Diagnostics;

namespace Latex.Invokers
{
    /// <summary>
    /// Invokes the LaTeX Compiler
    /// </summary>
    public abstract class AbstractInvoker
    {
        #region Public
        /// <summary>
        /// Gets or sets the folder containing the LaTeX executables
        /// </summary>
        public string Folder { get; set; }
        /// <summary>
        /// Checks if the necessary tools are installed
        /// </summary>
        /// <returns></returns>
        public abstract bool CheckTools();
        /// <summary>
        /// Invokes LaTeX
        /// </summary>
        /// <param name="inputFileName"></param>
        /// <returns></returns>
        public abstract InvokeResult Invoke(string inputFileName);
        #endregion

        #region Protected
        /// <summary>
        /// Extension of the output file
        /// </summary>
        protected string Extension = String.Empty;
        /// <summary>
        /// Cleans up LaTeX temporary files 
        /// </summary>
        /// <param name="temporaryFiles">Temporary files which specify the path and prefix of the files to be deleted</param>
        /// <returns></returns>
        protected void Clean(params string[] temporaryFiles)
        {
            var folders = new List<string>();
            var prefixes = new List<string>();
            foreach (var item in temporaryFiles)
            {
                var folder = Path.GetDirectoryName(item);
                if (!folders.Contains(folder))
                {
                    folders.Add(folder);
                }
                var prefix = Path.GetFileNameWithoutExtension(item);
                if (!prefixes.Contains(prefix))
                {
                    prefixes.Add(prefix);
                }
            }

            foreach (var folder in folders)
            {
                foreach (var item in Directory.GetFiles(folder, 
                    "*.*", SearchOption.TopDirectoryOnly))
                {
                    if (prefixes.Contains(Path.GetFileNameWithoutExtension(item)))
                    {
                        try
                        {
                            File.Delete(item);
                        }
                        catch (Exception) { }
                    }
                }
            }
        }
        /// <summary>
        /// Constructs a Process instance with the specified fileName and arguments
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        protected Process PrepareProcess(string fileName, string arguments)
        {
            var process = new Process();
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.WorkingDirectory = Path.GetTempPath();
            process.StartInfo.FileName = fileName;
            process.StartInfo.Arguments = arguments;

            return process;
        }
        /// <summary>
        /// Checks if the list of provided files exists in the LaTeX folder
        /// </summary>
        /// <param name="fileNames"></param>
        /// <returns></returns>
        protected bool QuickCheckToolFiles(params string[] fileNames)
        {
            foreach (var item in fileNames)
            {
                if (!File.Exists(Folder + "\\" + item))
                {
                    return false;
                }
            }
            return true;
        }
        #endregion
    }
}
