﻿using System;
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
        public SaveDirectory saveDirectory { get; set; }
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
            this.PopulateBackups(sender, e);
        }

        private void PopulateSaveGames(object sender, RoutedEventArgs e)
        {
            comboSaveSelect.Items.Clear();
            foreach (Save saveGame in this.saveDirectory.SaveGames)
                comboSaveSelect.Items.Add(saveGame);
            comboSaveSelect.SelectedIndex = 0;
        }

        private void PopulateBackups(object sender, RoutedEventArgs e)
        {
            this.backupDir = new BackupDirectory(this.saveDirectory.FullPath);
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
            this.saveDirectory = new SaveDirectory(saveDirPath);
            
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
            Save saveToBackUp;
            bool eraseAfter = (bool)checkBoxEraseAfter.IsChecked;
            bool timestamped = (bool)checkBoxTimestampArchive.IsChecked;
            bool browseAfter = false;
            try
            {
                saveToBackUp = (Save)this.comboSaveSelect.SelectedItem;
                string filename = saveToBackUp.ArchiveFilename(timestamped);

                if (File.Exists(filename))
                {
                    MessageBoxResult res = MessageBox.Show("Do you want to overwrite the existing backup?", "Backup Exists", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (res == MessageBoxResult.Yes)
                        File.Delete(filename);
                }

                saveToBackUp.Archive(eraseAfter, timestamped);
                if (eraseAfter)
                    this.PopulateSaveGames(sender, e);
                this.PopulateBackups(sender, e);
                if (browseAfter)
                    this.BrowseSaveDirectory(sender, e);
            }
            catch (NullReferenceException) { }
        }

        private void buttonRestore_Click(object sender, RoutedEventArgs e)
        {
            Backup selectedBackup = (Backup)this.comboBackupSelect.SelectedItem;
            string restoreAs = textBoxRestoreAs.Text;
            if (this.textBoxRestoreAs.Text == "")
            {
                MessageBox.Show("You must enter a name to restore this backup as.", "Error: No Restore-As Name", MessageBoxButton.OK, MessageBoxImage.Error);
                textBoxRestoreAs.Focus();
            }
            else
            {
                try
                {
                    selectedBackup.Restore(this.saveDirectory.FullPath, this.textBoxRestoreAs.Text, (bool)this.checkBoxRestoreOverwrite.IsChecked);
                    string msg = String.Format("Backup {0} restored as {1} successfully.", selectedBackup.Name, restoreAs);
                    MessageBox.Show(msg, "Restore successful");
                }
                catch(InvalidOperationException ex)
                {
                    if (ex.Message == "Invalid operation: save directory exists")
                        MessageBox.Show("Error restoring backup: save directory already exists.", "Error: Save Exists", MessageBoxButton.OK, MessageBoxImage.Error);
                    else
                        throw ex;
                }
            }
        }

        private void comboBackupSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.comboBackupSelect.SelectedItem != null)
                this.textBoxRestoreAs.Text = ((Backup)this.comboBackupSelect.SelectedItem).Name.Split('.').First();
        }

        private void BrowseSaveDirectory(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(this.saveDirectory.FullPath);
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


    }
}
