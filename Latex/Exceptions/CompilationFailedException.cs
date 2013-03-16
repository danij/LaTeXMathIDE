﻿/* LaTeX Math IDE
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

namespace Latex.Exceptions
{
    /// <summary>
    /// Gets thrown when a LaTeX compilation failes
    /// </summary>
    [Serializable]
    public class CompilationFailedException : Exception
    {
        public CompilationFailedException() { }
        public CompilationFailedException(string message) : base(message) { }
        public CompilationFailedException(string message, Exception inner) : base(message, inner) { }
        protected CompilationFailedException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
