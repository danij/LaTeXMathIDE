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
using Latex.Scheduler;
using Latex.Invokers;
using System.Threading;
using Latex.Decorators;
using System.IO;
using Image.Helpers;
using System.Windows.Media.Imaging;

namespace Latex.SymbolExporter
{
    class Processor : IDisposable
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public Processor(int resolution)
        {
            scheduler = new InvokeScheduler();
            scheduler.RequestFinished += new EventHandler<InvokeEventArgs>(scheduler_RequestFinished);
            invoker = new PNGInvoker()
            {
                Folder = @"d:\texlive\2010\bin\win32",
                Resolution = resolution
            };
            resetEvent = new ManualResetEvent(false);
        }
        /// <summary>
        /// Dispose the processor
        /// </summary>
        public void Dispose()
        {
            scheduler.Dispose();
        }
        /// <summary>
        /// Processes a LaTeX input like "\alpha", generates a transparent image
        /// out of it, saves the image in the Symbols folder and returns 
        /// the necessary xaml code
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public string Process(string input)
        {
            if (String.IsNullOrWhiteSpace(input)) return String.Empty;
            var decoratedInput = DocumentDecorator.DecorateFormulaOnly(
                MathDecorator.DecorateFormula(input));

            var request = new InvokeRequest()
            {
                Input = decoratedInput,
                Invoker = invoker,
            };

            resetEvent.Reset();
            scheduler.Add(request);
            resetEvent.WaitOne();

            var fileName = @"..\..\..\LatexMathIDE\Images\Symbols\" + GetFileName(input);
            var image = ImageTransparency.AddTransparencyToBlackAndWhiteImage(
                BitmapSourceHelper.Load(request.Result.Bytes));
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));
            
            using (var stream = new FileStream(fileName, FileMode.Create))
            {
                encoder.Save(stream);
            }
            
            return String.Format("<Fluent:GalleryItem Command=\"l:Commands.AddSymbol\" CommandParameter=\"{0}\" Width=\"32\" Height=\"32\"><Image Source=\"Images\\Symbols\\{1}\" Width=\"{2}\" Height=\"{3}\" /></Fluent:GalleryItem>",
                input, GetFileName(input), image.PixelWidth, image.PixelHeight);
        }
        /// <summary>
        /// Scheduler
        /// </summary>
        protected InvokeScheduler scheduler;
        protected PNGInvoker invoker;
        protected ManualResetEvent resetEvent;
        /// <summary>
        /// Gets the file name required to save a symbol
        /// Capital letters are replaced with _ + non-capital letter
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        protected string GetFileName(string input)
        {
            var builder = new StringBuilder();

            input = input.Replace("<", "_less_");
            input = input.Replace(">", "_more_");
            input = input.Replace("|", "_vline_");
            input = input.Replace("/", "_fslash_");
            input = input.Replace("(", "_obacket1_");
            input = input.Replace("[", "_obacket2_");
            input = input.Replace("{", "_obacket3_");
            input = input.Replace(")", "_cbacket1_");
            input = input.Replace("]", "_cbacket2_");
            input = input.Replace("}", "_cbacket3_");

            foreach (var item in input)
            {
                if (item == '\\') continue;
                if (item >= 'A' && item <= 'Z')
                {
                    builder.Append("_" + new string(item, 1).ToLower()[0]);
                }
                else
                {
                    builder.Append(item);
                }
            }
            builder.Append(".png");

            return builder.ToString();
        }


        #region Private
        /// <summary>
        /// Occurs when a request was processed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void scheduler_RequestFinished(object sender, InvokeEventArgs e)
        {
            resetEvent.Set();
        }
        #endregion
    }
}
