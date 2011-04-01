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
        private BackupDirectory backupDir { get; set; }
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
            this.backupDir = new BackupDirectory(saveDir.FullPath);
            comboBackupSelect.Items.Clear();
            foreach(Backup b in this.backupDir.Backups)
                comboBackupSelect.Items.Add(b);
            comboBackupSelect.SelectedIndex = 0;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.PathToDFExe == "" || !File.Exists(Properties.Settings.Default.PathToDFExe))
            {
                MessageBox.Show("Dwarf Fortress.exe not found. Please locate.");
                this.LocateDF2010(sender, e);
            }
            
            FileInfo fi = new FileInfo(Properties.Settings.Default.PathToDFExe);
            string saveDirPath = System.IO.Path.Combine(fi.DirectoryName, "data", "save");
            DFSaveDirectory saveDir = new DFSaveDirectory(saveDirPath);
            ((App)Application.Current).saveDirectory = saveDir;

            this.PopulateSaveGames(sender, e);
            this.PopulateBackups(sender, e);
        }

        private void menuExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
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
                this.PopulateBackups(sender, e);
            }
            catch (NullReferenceException) { }
        }

        private void buttonRestore_Click(object sender, RoutedEventArgs e)
        {
            Backup selectedBackup = (Backup)this.comboBackupSelect.SelectedItem;
            DFSaveDirectory saveDir = ((App)Application.Current).saveDirectory;
            string restoreAs = textBoxRestoreAs.Text;
            if (this.textBoxRestoreAs.Text == "")
            {
                MessageBox.Show("You must enter a name to restore this backup as.", "Error: No Restore-As Name", MessageBoxButton.OK, MessageBoxImage.Error);
                textBoxRestoreAs.Focus();
            }
            else
            {
                selectedBackup.Restore(saveDir.FullPath, this.textBoxRestoreAs.Text, (bool)this.checkBoxRestoreOverwrite.IsChecked);
                string msg = String.Format("Backup {0} restored as {1} successfully.", selectedBackup.Name, restoreAs);
                MessageBox.Show(msg, "Restore successful");
            }
        }

        private void comboBackupSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.textBoxRestoreAs.Text = ((Backup)this.comboBackupSelect.SelectedItem).Name.Replace(".zip","");
        }
    }
}
