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
using System.Windows.Input;

namespace Latex.MathIDE
{
    /// <summary>
    /// Provides commands
    /// </summary>
    static class Commands
    {
        public static RoutedUICommand Refresh =
            new RoutedUICommand("Refreshes the input", "Refresh", typeof(MainWindow));
        public static RoutedUICommand CopyResult =
            new RoutedUICommand("Copies the result image to the clipboard", "CopyResult", typeof(MainWindow));
        public static RoutedUICommand CopyInput =
            new RoutedUICommand("Copies the input to the clipboard", "CopyInput", typeof(MainWindow));
        public static RoutedUICommand CopyWholeInput =
            new RoutedUICommand("Copies the entire input document to the clipboard", "CopyWholeDocument", typeof(MainWindow));
        public static RoutedUICommand AddSymbol =
            new RoutedUICommand("Adds a symbol", "AddSymbol", typeof(MainWindow));
    }
}
