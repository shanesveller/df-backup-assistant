using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DFBackupAssistant
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<DFSave> saveGames { get; set; }
        public List<Backup> backups { get; set; }
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LocateDF2010(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "Dwarf Fortress"; // Default file name
            dlg.DefaultExt = ".exe"; // Default file extension
            dlg.Filter = "Applications (.exe)|*.exe"; // Filter files by extension

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                Properties.Settings.Default.PathToDFExe = dlg.FileName;
                Properties.Settings.Default.Save();
            }
            this.PopulateSaveGames(sender, e);
        }

        private void PopulateSaveGames(object sender, RoutedEventArgs e)
        {
            DFSaveDirectory saveDir = ((App)Application.Current).saveDirectory;
            comboSaveSelect.Items.Clear();
            foreach (DFSave saveGame in saveDir.SaveGames)
                comboSaveSelect.Items.Add(saveGame);
            comboSaveSelect.SelectedIndex = 0;
        }

        private void PopulateBackups(object sender, RoutedEventArgs e)
        {
            DFSaveDirectory saveDir = ((App)Application.Current).saveDirectory;

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.PathToDFExe == "" || !File.Exists(Properties.Settings.Default.PathToDFExe))
            {
                MessageBox.Show("Dwarf Fortress.exe not found. Please locate.");
                this.LocateDF2010(sender, e);
            }
            else
            {
                FileInfo fi = new FileInfo(Properties.Settings.Default.PathToDFExe);
                string saveDirPath = System.IO.Path.Combine(fi.DirectoryName, "data", "save");
                DFSaveDirectory saveDir = new DFSaveDirectory(saveDirPath);
                ((App)Application.Current).saveDirectory = saveDir;

                this.PopulateSaveGames(sender, e);
            }
        }

        private void buttonLaunchDF_Click(object sender, RoutedEventArgs e)
        {
            var startInfo = new ProcessStartInfo(Properties.Settings.Default.PathToDFExe);
            startInfo.WorkingDirectory = new FileInfo(Properties.Settings.Default.PathToDFExe).DirectoryName;
            Process.Start(startInfo);
        }

        private void buttonBackup_Click(object sender, RoutedEventArgs e)
        {
            DFSaveDirectory saveDir = ((App)Application.Current).saveDirectory;
            DFSave saveToBackUp;
            try
            {
                saveToBackUp = (DFSave)this.comboSaveSelect.SelectedItem;
                
                saveToBackUp.Archive((bool)checkBoxEraseAfter.IsChecked, (bool)checkBoxTimestampArchive.IsChecked);
                if((bool)checkBoxEraseAfter.IsChecked)
                    this.PopulateSaveGames(sender, e);
            }
            catch (NullReferenceException) { }
        }

        private void menuExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
