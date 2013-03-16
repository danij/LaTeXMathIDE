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
using System.Management;

namespace Latex.Helpers
{
    /// <summary>
    /// Provides information about the CPU
    /// </summary>
    static class CPUInfoHelper
    {
        /// <summary>
        /// Counts the number of CPU cores
        /// </summary>
        public static int CoreCount
        {
            get
            {
                int result = 0;
                var searcher = 
                    new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
                foreach (var item in searcher.Get())
                {
                    result += int.Parse(item["NumberOfCores"].ToString());
                }
                return result;
            }
        }
    }
}
