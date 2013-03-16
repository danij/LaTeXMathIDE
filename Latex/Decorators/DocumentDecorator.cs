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
    /// Adds LaTeX code to construct a document
    /// </summary>
    public class DocumentDecorator
    {
        /// <summary>
        /// Adds the necessary LaTeX code to begin a document without a full page
        /// </summary>
        /// <param name="input">The input</param>
        /// <param name="extendedSymbols">Use package amssymb</param>
        /// <returns></returns>
        public static string DecorateFormulaOnly(string input)
        {
            var builder = new StringBuilder();

            builder.AppendLine(@"\documentclass[preview]{standalone}");
            builder.AppendLine(@"\usepackage{amssymb, amsmath}");
            builder.AppendLine(@"\pagestyle{empty}");
            builder.AppendLine(@"\begin{document}");
            builder.AppendLine(input);
            builder.AppendLine(@"\end{document}");

            return builder.ToString();
        }
    }
}
