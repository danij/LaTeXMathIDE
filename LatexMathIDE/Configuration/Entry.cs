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
using System.IO;

namespace Latex.MathIDE.Configuration
{
    /// <summary>
    /// Holds configuration parameters
    /// </summary>
    class Entry : DependencyObject
    {
        #region Static
        public static readonly DependencyProperty AutoRefreshProperty =
            DependencyProperty.Register("AutoRefresh", typeof(bool),
            typeof(Entry), new UIPropertyMetadata(true));
        public static readonly DependencyProperty IsLaTeXFoundProperty =
            DependencyProperty.Register("IsLaTeXFound", typeof(bool),
            typeof(Entry), new UIPropertyMetadata(false));
        public static readonly DependencyProperty LaTeXFolderProperty =
            DependencyProperty.Register("LaTeXFolder", typeof(string), 
            typeof(Entry), new UIPropertyMetadata(String.Empty));
        #endregion
        #region Public
        /// <summary>
        /// Gets or sets whether to auto refresh the data
        /// </summary>
        public bool AutoRefresh
        {
            get { return (bool)GetValue(AutoRefreshProperty); }
            set { SetValue(AutoRefreshProperty, value); }
        }
        /// <summary>
        /// Checks if the required LaTeX executables have been found
        /// </summary>    
        public bool IsLaTeXFound
        {
            get { return (bool)GetValue(IsLaTeXFoundProperty); }
            protected set { SetValue(IsLaTeXFoundProperty, value); }
        }
        /// <summary>
        /// Gets or sets the LaTeXFolder
        /// </summary>
        public string LaTeXFolder
        {
            get { return (string)GetValue(LaTeXFolderProperty); }
            set 
            { 
                SetValue(LaTeXFolderProperty, value);
                var filesRequired = new string[] 
                {
                    "latex.exe", "dvipng.exe"
                };
                foreach (var item in filesRequired)
                {
                    if ( ! File.Exists(value + "\\" + item))
                    {
                        IsLaTeXFound = false;
                        return;
                    }
                }
                IsLaTeXFound = true;
            }
        }
        #endregion
    }
}
