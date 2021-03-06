﻿using System;
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

namespace DFBackupAssistant
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SaveDirectory savedir;
        FileSystemWatcher saveWatcher;
        public SaveDirectory saveDirectory {
            get { return savedir; }
            set
            {
                savedir = value;
                saveWatcher.Path = value.FullPath;
                saveWatcher.EnableRaisingEvents = true;
            }
        }
        public BackupDirectory backupDir { get; set; }
        
        public MainWindow()
        {
            InitializeComponent();

            saveWatcher = new FileSystemWatcher();
            saveWatcher.Filter = "world.sav";
            saveWatcher.IncludeSubdirectories = true;
            saveWatcher.Created += new FileSystemEventHandler(saveWatcher_ChangedCreatedOrDeleted);
            saveWatcher.Deleted += new FileSystemEventHandler(saveWatcher_ChangedCreatedOrDeleted);
            saveWatcher.Changed += new FileSystemEventHandler(saveWatcher_ChangedCreatedOrDeleted);
            saveWatcher.Renamed += new RenamedEventHandler(saveWatcher_Renamed);
        }

        #region Settings Handlers
        private void LocateDF2010(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Dwarf Fortress Backup Assistant doesn't know where your Dwarf Fortress is installed to.\nPlease locate Dwarf Fortress.exe.");
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "Dwarf Fortress"; // Default file name
            dlg.DefaultExt = ".exe"; // Default file extension
            dlg.Filter = "Applications (.exe)|*.exe"; // Filter files by extension

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if ((bool)result)
            {
                // Open document
                Properties.Settings.Default.PathToDFExe = dlg.FileName;
                Properties.Settings.Default.Save();
            }
        }

        private void LocateBackupDirectory(object sender, RoutedEventArgs e)
        {
            var folderBrowserDlg = new System.Windows.Forms.FolderBrowserDialog();
            MessageBox.Show("Please select the folder to store backups in.");
            folderBrowserDlg.ShowNewFolderButton = true;
            if (Properties.Settings.Default.BackupFolder != "")
                folderBrowserDlg.SelectedPath = Properties.Settings.Default.BackupFolder;
            else
                folderBrowserDlg.SelectedPath = Directory.GetCurrentDirectory();
            System.Windows.Forms.DialogResult res = folderBrowserDlg.ShowDialog();
            if (res == System.Windows.Forms.DialogResult.OK)
            {
                MessageBox.Show("New backups will be saved and loaded from:\n" + folderBrowserDlg.SelectedPath);

                Properties.Settings.Default.BackupFolder = folderBrowserDlg.SelectedPath;
                Properties.Settings.Default.Save();

                this.backupDir = new BackupDirectory(Properties.Settings.Default.BackupFolder);
                this.PopulateBackups(sender, e);
            }
        }
        #endregion

        private void PopulateSaveGames(object sender, RoutedEventArgs e)
        {
            this.saveDirectory.RefreshSaveGames();
            comboSaveSelect.Items.Clear();
            foreach (Save saveGame in this.saveDirectory.SaveGames)
                comboSaveSelect.Items.Add(saveGame);
            comboSaveSelect.SelectedIndex = 0;
        }

        private void PopulateBackups(object sender, RoutedEventArgs e)
        {
            this.backupDir.RefreshBackups();
            comboBackupSelect.Items.Clear();
            foreach(Backup b in this.backupDir.Backups)
                comboBackupSelect.Items.Add(b);
            comboBackupSelect.SelectedIndex = 0;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.PathToDFExe == "" || !File.Exists(Properties.Settings.Default.PathToDFExe))
                this.LocateDF2010(sender, e);

            if (Properties.Settings.Default.BackupFolder == "" || !Directory.Exists(Properties.Settings.Default.BackupFolder))
                this.LocateBackupDirectory(sender, e);

            FileInfo fi = new FileInfo(Properties.Settings.Default.PathToDFExe);
            string saveDirPath = System.IO.Path.Combine(fi.DirectoryName, "data", "save");
            this.saveDirectory = new SaveDirectory(saveDirPath);
            this.backupDir = new BackupDirectory(Properties.Settings.Default.BackupFolder);
            
            this.PopulateSaveGames(sender, e);
            this.PopulateBackups(sender, e);
        }

        #region Menu Handlers
        private void menuExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void BrowseSaveDirectory(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(this.saveDirectory.FullPath);
        }

        private void menuLaunchDF_Click(object sender, RoutedEventArgs e)
        {
            var startInfo = new ProcessStartInfo(Properties.Settings.Default.PathToDFExe);
            startInfo.WorkingDirectory = new FileInfo(Properties.Settings.Default.PathToDFExe).DirectoryName;
            Process.Start(startInfo);
        }
        #endregion

        #region Button Handlers
        private void buttonBackup_Click(object sender, RoutedEventArgs e)
        {
            Save saveToBackUp;
            bool eraseAfter = (bool)checkBoxEraseAfter.IsChecked;
            bool timestamped = (bool)checkBoxTimestampArchive.IsChecked;
            bool browseAfter = false;
            try
            {
                saveToBackUp = (Save)this.comboSaveSelect.SelectedItem;
                string filename = saveToBackUp.ArchiveFilename(this.backupDir, timestamped);

                if (File.Exists(filename))
                {
                    MessageBoxResult res = MessageBox.Show("Do you want to overwrite the existing backup?", "Backup Exists", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (res == MessageBoxResult.Yes)
                        File.Delete(filename);
                }

                saveToBackUp.Archive(this.backupDir, eraseAfter, timestamped);
            }
            finally
            {
                this.PopulateSaveGames(sender, e);
                this.PopulateBackups(sender, e);
                if (browseAfter)
                    this.BrowseSaveDirectory(sender, e);
            }

        }

        private void buttonRestore_Click(object sender, RoutedEventArgs e)
        {
            Backup selectedBackup = (Backup)this.comboBackupSelect.SelectedItem;
            string restoreAs = textBoxRestoreAs.Text;
            string targetDirectory = System.IO.Path.Combine(this.saveDirectory.FullPath, restoreAs);
            bool overwrite = false;

            if (selectedBackup == null)
            {
                MessageBox.Show("No backup file selected.", "Error: No Backup File Selected", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (this.textBoxRestoreAs.Text == "")
            {
                MessageBox.Show("You must enter a folder name to restore this backup as.", "Error: No Restore-As Name", MessageBoxButton.OK, MessageBoxImage.Error);
                textBoxRestoreAs.Focus();
                return;
            }
            
            if ((bool)checkBoxRestoreOverwrite.IsChecked)
                overwrite = true;
            else if (Directory.Exists(targetDirectory))
            {
                MessageBoxResult res = MessageBox.Show("Overwrite existing savegame?", "Save Game Exists", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (res == MessageBoxResult.Yes)
                    overwrite = true;
            }

            selectedBackup.Restore(System.IO.Path.Combine(this.saveDirectory.FullPath, this.textBoxRestoreAs.Text), overwrite);
            string msg = String.Format("Backup {0} restored as {1} successfully.", selectedBackup.Name, restoreAs);
            MessageBox.Show(msg, "Restore Successful", MessageBoxButton.OK, MessageBoxImage.Information);
            this.PopulateBackups(sender, e);
            this.PopulateSaveGames(sender, e);
        }
        #endregion

        #region Other Control Handlers
        private void comboBackupSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.comboBackupSelect.SelectedItem != null)
                this.textBoxRestoreAs.Text = ((Backup)this.comboBackupSelect.SelectedItem).Name.Split('.').First();
        }

        private void UpdateBackupFilenameTextBox(object sender, RoutedEventArgs e)
        {
            this.UpdateBackupFilenameTextBox();
        }

        private void UpdateBackupFilenameTextBox(object sender, SelectionChangedEventArgs e)
        {
            this.UpdateBackupFilenameTextBox();
        }

        private void UpdateBackupFilenameTextBox()
        {
            if (this.comboSaveSelect.SelectedItem != null)
            {
                this.textBoxBackupFilename.Text = this.comboSaveSelect.SelectedItem.ToString();
                if ((bool)this.checkBoxTimestampArchive.IsChecked)
                    this.textBoxBackupFilename.Text += DateTime.Now.ToString(".yyyyMMdd-HHmm");
                this.textBoxBackupFilename.Text += ".zip";
            }
        }
        #endregion
        
        #region FileSystemWatcherHandlers
        private void saveWatcher_ChangedCreatedOrDeleted(object sender, FileSystemEventArgs e)
        {
            this.PopulateSaveGames(sender, null);
        }

        private void saveWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            this.PopulateSaveGames(sender, null);
        }
        #endregion
    }
}
