using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.IO;
using System.ComponentModel;
using System.Reflection;
using LogViewer.Infrastructure;
using LogViewer.Model;
using System.Windows.Interop;
using System.Drawing;


namespace LogViewer
{
    public partial class OverviewWindow : Window
    {
        private readonly FileLogEntryController _filec;

        public ILogSource[] LogSources { get; set; }

        public OverviewWindow()
        {
            _filec = new FileLogEntryController();
            //An aggregate catalog that combines multiple catalogs
            var catalog = new AggregateCatalog();
            //Adds all the parts found in the same assembly as the Program class
            catalog.Catalogs.Add(new AssemblyCatalog(GetType().Assembly));

            //Create the CompositionContainer with the parts in the catalog
            var container = new CompositionContainer(catalog);
            Title = string.Format("LogViewer  v.{0}", Assembly.GetExecutingAssembly().GetName().Version);
            Loaded += OverviewWindow_Loaded;
            Closed += OverviewWindow_Closed;
            LogSources = container.GetExports<ILogSource>().Select(src => src.Value).ToArray();

            Icon = Imaging.CreateBitmapSourceFromHIcon(SystemIcons.Question.Handle, Int32Rect.Empty, null);

            InitializeComponent();
            DataContext = _filec;
            _filec.FileNameChanged += ObservableFileName_PropertyChanged;
        }

        void ObservableFileName_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ((App)App.Current).AddFilenameToRecent(_filec.FileName);
        }

        void OverviewWindow_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void OverviewWindow_Loaded(object sender, RoutedEventArgs args) 
        {
            if (((App)App.Current).Args.Any() && File.Exists(((App)App.Current).Args.First()))
            {
                this._filec.FileName = ((App)App.Current).Args.First();
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
        }

        private void MenuFileOpen_Click(object sender, RoutedEventArgs e)
        {
            var oOpenFileDialog = new System.Windows.Forms.OpenFileDialog();
            if (oOpenFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this._filec.FileName = oOpenFileDialog.FileName;
            }
        }

        private void MenuFileExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void textBoxFind_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (textBoxFind.Text.Length > 0)
                {
                    _filec.SelectNextEntry(entry => entry.Message.Contains(textBoxFind.Text));
                }
            }
        }

        private void RecentMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var context = ((System.Windows.Controls.MenuItem)sender).DataContext as RecentFile;
            if (null != context)
            {
                this._filec.FileName = context.Filepath;
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                case Key.Left:
                    _filec.SelectPreviousEntry(entry => true);
                    break;
                case Key.Down:
                case Key.Right:
                    _filec.SelectNextEntry(entry => true);
                    break;
                case Key.PageDown:
                    //GetNextPage
                    break;
                case Key.PageUp:
                    //GetPreviousPage
                    break;
                case Key.Home:
                    _filec.SelectTop();
                    break;
                case Key.End:
                    _filec.SelectBottom();
                    break;
                default:
                    break;
            }
        }

    }
}
