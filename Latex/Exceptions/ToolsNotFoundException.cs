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

namespace Latex.Exceptions
{
    /// <summary>
    /// Gets thrown when the LaTeX tools are not found
    /// </summary>
    [Serializable]
    public class ToolsNotFoundException : Exception
    {
        public ToolsNotFoundException() { }
        public ToolsNotFoundException(string message) : base(message) { }
        public ToolsNotFoundException(string message, Exception inner) : base(message, inner) { }
        protected ToolsNotFoundException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
