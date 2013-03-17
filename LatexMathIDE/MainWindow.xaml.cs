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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Latex;
using Latex.Invokers;
using Latex.Helpers;
using Latex.Decorators;
using Image.Helpers;
using Latex.Scheduler;
using Microsoft.Win32;

namespace Latex.MathIDE
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    partial class MainWindow : Fluent.RibbonWindow
    {
        #region Public
        /// <summary>
        /// Default constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            ViewModel = new ViewModel();
            DataContext = ViewModel;
            inputTextBox.Focus();
            if ( ! ViewModel.ConfigurationEntry.IsLaTeXFound)
            {
                SelectLatexFolder();                
            }
        }
        /// <summary>
        /// Gets or sets the view model
        /// </summary>
        public ViewModel ViewModel { get; protected set; }
        #endregion

        #region Private
        /// <summary>
        /// Occurs when the window is closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_Closed(object sender, EventArgs e)
        {
            ViewModel.SaveConfiguration();
            ViewModel.Dispose();
        }
        /// <summary>
        /// Occurs when a key is pressed in the input text box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void inputTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (ViewModel.ConfigurationEntry.AutoRefresh)
            {
                ViewModel.ConvertCurrentInput();
            }            
        }
        /// <summary>
        /// Checks if the Refresh command can be executed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefreshCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ViewModel.ConfigurationEntry.IsLaTeXFound;
            e.Handled = true;
        }
        /// <summary>
        /// Handles the Refresh command
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefreshCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ViewModel.ConvertCurrentInput();
            e.Handled = true;
        }
        /// <summary>
        /// Checks if the CopyResult command can be executed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CopyResultCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ViewModel.ResultImage != null;
            e.Handled = true;
        }
        /// <summary>
        /// Handles the CopyResult command
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CopyResultCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ViewModel.CopyResult();
            e.Handled = true;
        }
        /// <summary>
        /// Checks if the CopyInput command can be executed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CopyInputCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ! String.IsNullOrWhiteSpace(ViewModel.TextInput);
            e.Handled = true;
        }
        /// <summary>
        /// Handles the CopyInput command
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CopyInputCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ViewModel.CopyInput();
            e.Handled = true;
        }
        /// <summary>
        /// Checks if the CopyWholeInput command can be executed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CopyWholeInputCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !String.IsNullOrWhiteSpace(ViewModel.TextInput);
            e.Handled = true;
        }
        /// <summary>
        /// Handles the CopyInput command
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CopyWholeInputCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ViewModel.CopyWholeInput();
            e.Handled = true;
        }
        /// <summary>
        /// Handles the Help command
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HelpCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                Process.Start(@"http://en.wikibooks.org/wiki/LaTeX/Mathematics");
            }
            catch (Exception) { }
            e.Handled = true;
        }
        /// <summary>
        /// Checks if the AddSymbol command can be executed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddSymbolCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }
        /// <summary>
        /// Handles the AddSymbol command
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddSymbolCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter == null) return;

            inputTextBox.SelectedText = e.Parameter.ToString() + " ";
            inputTextBox_KeyUp(inputTextBox, null);
            e.Handled = true;
        }       
        /// <summary>
        /// Checks if the SaveAs command can be executed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveAsCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ViewModel.ResultImageTransparent != null;
            e.Handled = true;
        }
        /// <summary>
        /// Handles the SaveAs command
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = new SaveFileDialog();
            dialog.Filter = "PNG Image|*.png";
            dialog.Title = "Save As...";
            dialog.OverwritePrompt = true;
            dialog.ValidateNames = true;
            dialog.AddExtension = true;

            if (dialog.ShowDialog().Value)
            {
                var fileName = dialog.FileName;
                try
                {
                    var encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(ViewModel.ResultImageTransparent));
                    using (var stream = new FileStream(fileName, FileMode.Create))
                    {
                        encoder.Save(stream);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            e.Handled = true;
        }
        /// <summary>
        /// Displays a dialog prompting the user the select the LaTeX executables folder
        /// </summary>
        private void SelectLatexFolder()
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.Description = @"Select the LaTeX folder (e.g. ..\bin\win32)";
            dialog.SelectedPath = ViewModel.ConfigurationEntry.LaTeXFolder;
            dialog.ShowNewFolderButton = false;
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ViewModel.ConfigurationEntry.LaTeXFolder = dialog.SelectedPath;
                if (!ViewModel.ConfigurationEntry.IsLaTeXFound)
                {
                    if (MessageBox.Show("The required LaTeX executables were not found in the specified folder!" +
                        Environment.NewLine + "Retry?", "Error",
                        MessageBoxButton.YesNo, MessageBoxImage.Error)
                        == MessageBoxResult.Yes)
                    {
                        SelectLatexFolder();
                    }
                }
                else
                {
                    ViewModel.SaveConfiguration();
                }
            }
            if (!ViewModel.ConfigurationEntry.IsLaTeXFound)
            {
                ViewModel.SignalLatexNotFound();
            }
        }
        /// <summary>
        /// Handles the AutoRefresh CheckBox checked event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void autoRefreshCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            //TODO: binding only works one way
            ViewModel.ConfigurationEntry.AutoRefresh = true;
        }
        /// <summary>
        /// Handles the AutoRefresh CheckBox unchecked event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void autoRefreshCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            //TODO: binding only works one way
            ViewModel.ConfigurationEntry.AutoRefresh = false;
        }      
        /// <summary>
        /// Handles the Resolution ComboBox selection changed event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void resolutionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //TODO: binding only works one way
            if (resolutionComboBox.SelectedValue == null) return;
            ViewModel.Resolution = Convert.ToInt32(resolutionComboBox.SelectedValue);
        }       
        /// <summary>
        /// Handles the LaTeX Folder Button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LaTeXFolderButton_Click(object sender, RoutedEventArgs e)
        {
            SelectLatexFolder();
        }
        /// <summary>
        /// Occurs when the mouse is clicked inside the donate image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void donateStatusBarItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Process.Start(@"https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=WQQ9XBUKJ4WTN");
            }
            catch (Exception) { }
            e.Handled = true;
        }
        #endregion
    }
}
