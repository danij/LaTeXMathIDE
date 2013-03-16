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

namespace Latex.Decorators
{
    /// <summary>
    /// Adds LaTeX code to decorate math formulas
    /// </summary>
    public class MathDecorator
    {
        /// <summary>
        /// Adds LaTeX code to encapsulate a math formula
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string DecorateFormula(string value)
        {
            return @"\begin{gather*}" + value + @"\end{gather*}";
        }
        /// <summary>
        /// Removes empty lines and suffixes each line with \\
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string DecorateNewLines(string value)
        {
            return String.Join(@"\\" + Environment.NewLine, 
                value.Split(new string[] {Environment.NewLine}, 
                StringSplitOptions.RemoveEmptyEntries));
        }
    }
}
