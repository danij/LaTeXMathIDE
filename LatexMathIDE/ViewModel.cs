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
using Latex.Invokers;
using Latex.Scheduler;
using System.Threading;
using System.Windows.Media.Imaging;
using System.IO;
using Latex.Helpers;
using System.Windows.Media;
using System.Windows.Input;
using Latex.Decorators;
using System.Windows.Shell;
using Image.Helpers;

namespace Latex.MathIDE
{
    /// <summary>
    /// Default view model
    /// </summary>
    class ViewModel : DependencyObject, IDisposable
    {
        #region Static
        public static readonly DependencyProperty ConfigurationEntryProperty =
            DependencyProperty.Register("ConfigurationEntry", 
            typeof(Configuration.Entry), typeof(ViewModel), 
            new UIPropertyMetadata(null));
        public static readonly DependencyProperty ProgressStateProperty =
            DependencyProperty.Register("ProgressState", typeof(TaskbarItemProgressState),
            typeof(ViewModel), new UIPropertyMetadata(TaskbarItemProgressState.None));
        public static readonly DependencyProperty ResolutionProperty =
            DependencyProperty.Register("Resolution", typeof(int),
            typeof(ViewModel), new UIPropertyMetadata(300));
        public static readonly DependencyProperty ResultImageProperty =
            DependencyProperty.Register("ResultImage", typeof(BitmapSource),
            typeof(ViewModel), new UIPropertyMetadata(null));
        public static readonly DependencyProperty ResultImageTransparentProperty =
            DependencyProperty.Register("ResultImageTransparent", typeof(BitmapSource),
            typeof(ViewModel), new UIPropertyMetadata(null));
        public static readonly DependencyProperty StatusStringProperty =
            DependencyProperty.Register("StatusString", typeof(string),
            typeof(ViewModel), new UIPropertyMetadata("Ready"));
        public static readonly DependencyProperty TextInputProperty =
            DependencyProperty.Register("TextInput", typeof(string),
            typeof(ViewModel), new UIPropertyMetadata(String.Empty));
        public static readonly DependencyProperty ViewCursorProperty =
            DependencyProperty.Register("ViewCursor", typeof(Cursor),
            typeof(ViewModel), new UIPropertyMetadata(Cursors.Arrow));
        #endregion

        #region Public
        /// <summary>
        /// Default constructor
        /// </summary>
        public ViewModel()
        {
            Resolutions = new int[]
            {
                50, 100, 200, 300, 400, 500, 600, 800, 1000, 1200
            };

            ConfigurationEntry = manager.Load();
            if ( ! ConfigurationEntry.IsLaTeXFound)
            {
                SignalLatexNotFound();
            }

            Invoker = new PNGInvoker();
            Invoker.Folder = ConfigurationEntry.LaTeXFolder;
            Invoker.Resolution = Resolution;

            Scheduler.RequestFinished +=
                new EventHandler<InvokeEventArgs>(scheduler_RequestFinished);
        }
        /// <summary>
        /// Gets the configuration entry
        /// </summary>
        public Configuration.Entry ConfigurationEntry
        {
            get { return (Configuration.Entry)GetValue(ConfigurationEntryProperty); }
            protected set { SetValue(ConfigurationEntryProperty, value); }
        }        
        /// <summary>
        /// Gets the invoker
        /// </summary>
        public PNGInvoker Invoker { get; protected set; }
        /// <summary>
        /// Gets whether the view model is disposed
        /// </summary>
        public bool IsDisposed { get; protected set; }        
        /// <summary>
        /// Gets the current status string
        /// </summary>
        public string StatusString
        {
            get { return (string)GetValue(StatusStringProperty); }
            protected set { SetValue(StatusStringProperty, value); }
        }
        /// <summary>
        /// Gets the current progress state to display
        /// </summary>
        public TaskbarItemProgressState ProgressState
        {
            get { return (TaskbarItemProgressState)GetValue(ProgressStateProperty); }
            protected set { SetValue(ProgressStateProperty, value); }
        }
        /// <summary>
        /// Gets or sets the current resolution
        /// </summary>
        public int Resolution
        {
            get { return (int)GetValue(ResolutionProperty); }
            set 
            { 
                SetValue(ResolutionProperty, value);
                Invoker.Resolution = value;
                ConvertCurrentInput();
            }
        }       
        /// <summary>
        /// Gets or sets the predefined resolutions 
        /// </summary>
        public IEnumerable<int> Resolutions { get; protected set; }
        /// <summary>
        /// Gets the result image
        /// </summary>
        public BitmapSource ResultImage
        {
            get { return (BitmapSource)GetValue(ResultImageProperty); }
            set { SetValue(ResultImageProperty, value); }
        }
        /// <summary>
        /// Gets the transparent result image
        /// </summary>
        public BitmapSource ResultImageTransparent
        {
            get { return (BitmapSource)GetValue(ResultImageTransparentProperty); }
            set { SetValue(ResultImageTransparentProperty, value); }
        }        
        /// <summary>
        /// Gets or sets the text input
        /// </summary>
        public string TextInput
        {
            get { return (string)GetValue(TextInputProperty); }
            set { SetValue(TextInputProperty, value); }
        }
        /// <summary>
        /// Gets or sets the cursor to use in the view
        /// </summary>
        public Cursor ViewCursor
        {
            get { return (Cursor)GetValue(ViewCursorProperty); }
            set { SetValue(ViewCursorProperty, value); }
        }
        /// <summary>
        /// Disposes the object
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Converts the current LaTeX input to an image
        /// </summary>
        public void ConvertCurrentInput()
        {
            if ( ! ConfigurationEntry.IsLaTeXFound)
            {
                return;
            }
            ViewCursor = Cursors.Wait;

            Scheduler.ClearPendingRequests();            

            LatestRequest = new InvokeRequest()
            {
                Invoker = Invoker,
                Input = DecorateInput(),
                Identifier = TextInput
            };

            Scheduler.Add(LatestRequest);
            ProgressState = TaskbarItemProgressState.Indeterminate;
            StatusString = "Working...";

            ViewCursor = Cursors.Arrow;
        }
        /// <summary>
        /// Copies the result to clipboard
        /// </summary>
        public void CopyResult()
        {
            if (ResultImage != null)
            {
                Clipboard.SetImage(ResultImage);
            }
        }
        /// <summary>
        /// Copies the input to clipboard
        /// </summary>
        public void CopyInput()
        {
            if ( ! String.IsNullOrEmpty(TextInput))
            {
                Clipboard.SetText(TextInput);
            }
        }
        /// <summary>
        /// Copies the whole input to clipboard
        /// </summary>
        public void CopyWholeInput()
        {
            if ( ! String.IsNullOrEmpty(TextInput))
            {
                Clipboard.SetText(DecorateInput());
            }
        }
        /// <summary>
        /// Saves the current configuration to disc
        /// </summary>
        public void SaveConfiguration()
        {
            if (ConfigurationEntry.IsLaTeXFound)
            {
                Invoker.Folder = ConfigurationEntry.LaTeXFolder;
                ConvertCurrentInput();
            }
            manager.Save(ConfigurationEntry);
        }
        /// <summary>
        /// Signals that the required LaTeX executables were not found
        /// </summary>
        public void SignalLatexNotFound()
        {
            if (String.IsNullOrEmpty(ConfigurationEntry.LaTeXFolder))
            {
                StatusString = "LaTeX folder was not configured";
            }
            else
            {
                StatusString = "LaTeX was not found";
            }
            ProgressState = TaskbarItemProgressState.Error;
        }
        #endregion

        #region Protected
        /// <summary>
        /// Configuration manager
        /// </summary>
        protected Configuration.Manager manager = new Configuration.Manager();
        /// <summary>
        /// Scheduler
        /// </summary>
        protected InvokeScheduler Scheduler = new InvokeScheduler();
        /// <summary>
        /// The latest invoke request
        /// </summary>
        protected InvokeRequest LatestRequest;
        /// <summary>
        /// Decorates the input
        /// </summary>
        /// <returns>A LaTeX document</returns>
        protected string DecorateInput()
        {
            var input = MathDecorator.DecorateFormula(
                MathDecorator.DecorateNewLines(TextInput));
            return DocumentDecorator.DecorateFormulaOnly(input);
        }
        /// <summary>
        /// Occurs when a request has processed 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void scheduler_RequestFinished(object sender, InvokeEventArgs e)
        {
            if (e.Request != LatestRequest)
            {
                return;
            }
            Dispatcher.BeginInvoke((ThreadStart)delegate
            {
                try
                {
                    if (e.Request.Result.Exception != null)
                    {
                        ProgressState = TaskbarItemProgressState.Error;
                        StatusString = e.Request.Result.Exception.Message;
                    }
                    else
                    {
                        ResultImage = BitmapSourceHelper.Load(e.Request.Result.Bytes);
                        ResultImageTransparent = 
                            ImageTransparency.AddTransparencyToBlackAndWhiteImage(ResultImage);
                        ProgressState = TaskbarItemProgressState.None;
                        StatusString = "Ready";
                    }                    
                }
                catch (Exception)
                {
                    ResultImage = null;
                    ResultImageTransparent = null;
                    ProgressState = TaskbarItemProgressState.Error;
                }
                CommandManager.InvalidateRequerySuggested();
            });
        }
        /// <summary>
        /// Disposes the object
        /// </summary>
        /// <param name="disposing"></param>
        protected void Dispose(bool disposing)
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException("ViewModel");
            }
            if (disposing)
            {
                Scheduler.Dispose();
                IsDisposed = true;
            }
            //no unmanaged resource to dispose
        }
        #endregion
    }
}
