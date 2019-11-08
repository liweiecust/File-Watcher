using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FileWatcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private FileSystemWatcher watcher;
        public MainWindow()
        {
            InitializeComponent();
            watcher = new FileSystemWatcher();
            watcher.Deleted += (s, e) => AddMessage(e.FullPath, "Deleted");
            watcher.Renamed += (s, e) => RenameMessage(e.OldName, $"renamed as {e.FullPath}");
            watcher.Changed += (s, e) => AddMessage(e.FullPath, e.ChangeType.ToString());
            watcher.Created += (s, e) => AddMessage(e.FullPath, "Created");
        }

        private void LocationBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            WatchButton.IsEnabled = !String.IsNullOrEmpty(LocationBox.Text);
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            bool? folder = rdlFolder.IsChecked;
            if (folder == null || folder == true)
            {
                FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
                DialogResult result = folderBrowser.ShowDialog();
                if(result==System.Windows.Forms.DialogResult.Cancel)
                {
                    return;
                }
                LocationBox.Text = folderBrowser.SelectedPath.Trim();
            }
            else
            {
                Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
                if (dialog.ShowDialog(this) == true)
                {
                    LocationBox.Text = dialog.FileName;
                }
            }
          
        }

        private void WatchButton_Click(object sender, RoutedEventArgs e)
        {
            if(WatchButton.Content.ToString()=="Watch")
            {
                bool? folder = rdlFolder.IsChecked;
                if (folder == null || folder == true)
                {
                    watcher.Path = LocationBox.Text;
                    //watcher.Filter = System.IO.Path.GetFileName(LocationBox.Text);
                }
                else
                {
                    watcher.Path = System.IO.Path.GetDirectoryName(LocationBox.Text);
                    watcher.Filter = System.IO.Path.GetFileName(LocationBox.Text);
                }
                watcher.IncludeSubdirectories = true;
                watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.Size|NotifyFilters.CreationTime|NotifyFilters.DirectoryName;
                DitributeMessage(LocationBox.Text,"Watching " + LocationBox.Text);
                watcher.EnableRaisingEvents = true;
                WatchButton.Content = "Stop";

            }
            else
            {
                watcher.EnableRaisingEvents = false;
                WatchButton.Content = "Watch";
            }
          

        }
        public void AddMessage(string path,string type)
        {
            string message = string.Format("{0} File: {1} {2}",DateTime.Now.ToString(),path,type);
            DitributeMessage(path, message);
        }
        public void RenameMessage(string path, string newPath)
        {
            string message = string.Format("{0} File: {1} Renamed as {1}", path, newPath);
            DitributeMessage(path, message);
        }
        public void DitributeMessage(string path,string message)
        {
            string version = "";
            if (File.Exists(path))
            {
                version = System.Diagnostics.FileVersionInfo.GetVersionInfo(path).FileVersion;
                if(version!=null)
                {
                    version = string.Format("File version: {0}", version);
                }
                
            }

            Dispatcher.BeginInvoke(new Action(() => WatchOutPut.Items.Insert(0, new VisualFile(message, version))));
        }
        
    }
}
