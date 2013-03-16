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
using Latex.Invokers;

namespace Latex.Scheduler
{
    /// <summary>
    /// Stores what should be invoked and the current status of the request
    /// </summary>
    public class InvokeRequest
    {
        /// <summary>
        /// The status of an invoke request
        /// </summary>
        public enum InvokeStatus
        {
            Pending,
            Running,
            Finished
        }
        /// <summary>
        /// Gets or sets what to invoke
        /// </summary>
        public AbstractInvoker Invoker { get; set; }
        /// <summary>
        /// Gets or sets the file name of the LaTeX file
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// Gets or sets the LaTeX input, in case the file is not specified
        /// </summary>
        public string Input { get; set; }
        /// <summary>
        /// Gets or sets the requests identifier (used for caching)
        /// </summary>
        public string Identifier { get; set; }
        /// <summary>
        /// Gets or sets the status of the invoke
        /// </summary>
        public InvokeStatus Status { get; set; }
        /// <summary>
        /// Gets or sets the result of the invoke
        /// </summary>
        public InvokeResult Result { get; set; }
    }
}
