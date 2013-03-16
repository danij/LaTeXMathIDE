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

namespace Latex.MathIDE.Configuration
{
    /// <summary>
    /// Stores and loads the configuration from disk
    /// </summary>
    class Manager
    {
        #region Public
        /// <summary>
        /// Default constructor
        /// </summary>
        public Manager()
        {
            path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + 
                @"\LaTeXMathIDE\settings.txt";
        }
        /// <summary>
        /// Loads the configuration from disc
        /// </summary>
        /// <returns></returns>
        public Entry Load()
        {
            var result = new Entry();
            if (File.Exists(path))
            {
                try
                {
                    var lines = File.ReadAllLines(path);
                    foreach (var line in lines.Select(l => l.Trim()))
                    {
                        if (String.IsNullOrEmpty(line) ||
                            line[0] == '#' ||
                            ! line.Contains('=')) continue;
                        var index = line.IndexOf('=');
                        var key = line.Substring(0, index).ToLower();
                        var value = line.Substring(index + 1);

                        switch (key)
                        {
                            case "latexfolder":
                                result.LaTeXFolder = value;
                                break;
                            case "autorefresh":
                                result.AutoRefresh = value.ToLower() == "true";
                                break;
                            default:
                                break;
                        }
                    }
                }
                catch (Exception) { }
            }

            return result;
        }
        /// <summary>
        /// Saves the configuration to disc
        /// </summary>
        /// <param name="entry"></param>
        public void Save(Entry entry)
        {
            if (entry == null)
            {
                throw new ArgumentNullException("entry");
            }
            var folder = Path.GetDirectoryName(path);
            if ( ! Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            using (var writer = new StreamWriter(path, false))
            {
                writer.WriteLine("LaTeXFolder=" + entry.LaTeXFolder);
                writer.WriteLine("AutoRefresh=" + 
                    (entry.AutoRefresh ? "true" : "false"));
            }
        }
        #endregion

        #region Protected
        /// <summary>
        /// Stores the path of the configuration file
        /// </summary>
        protected string path;
        #endregion
    }
}
