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
            var d = Directory.GetLogicalDrives();
            InitializeComponent();
            watcher = new FileSystemWatcher();
            watcher.Deleted += (s, e) => AddMessage($"File: {e.FullPath} Deleted.");
            watcher.Renamed += (s, e) => AddMessage($"File: renamed from {e.OldName} to {e.FullPath} ");
            watcher.Changed += (s, e) => AddMessage($"File: {e.FullPath} {e.ChangeType.ToString()}");
            watcher.Created += (s, e) => AddMessage($"File: {e.FullPath} Created.");
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
                    watcher.Filter = System.IO.Path.GetFileName(LocationBox.Text);
                }
                else
                {
                    watcher.Path = System.IO.Path.GetDirectoryName(LocationBox.Text);
                    watcher.Filter = System.IO.Path.GetFileName(LocationBox.Text);
                }
                watcher.IncludeSubdirectories = true;
                watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.Size;
                AddMessage("Watching " + LocationBox.Text);
                watcher.EnableRaisingEvents = true;
                WatchButton.Content = "Stop";

            }
            else
            {
                watcher.EnableRaisingEvents = false;
                WatchButton.Content = "Watch";
            }
          

        }
        public void AddMessage(string message)
        {
            Dispatcher.BeginInvoke(new Action(() => WatchOutPut.Items.Insert(0, DateTime.Now.ToString()+" "+ message)));
        }
    }
}
