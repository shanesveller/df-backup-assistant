using System;
using System.Collections.Generic;
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
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Locate7zip(object sender, EventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "7za"; // Default file name
            dlg.DefaultExt = ".exe"; // Default file extension
            dlg.Filter = "Applications (.exe)|*.exe"; // Filter files by extension

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                Properties.Settings.Default.PathTo7zaExe = dlg.FileName;
                Properties.Settings.Default.Save();
            }
        }

        private void LocateDF2010(object sender, EventArgs e)
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
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.PathTo7zaExe == "" || !File.Exists(Properties.Settings.Default.PathTo7zaExe))
            {
                MessageBox.Show("7za.exe not found. Please locate.");
                this.Locate7zip(sender, e);
            }
            if (Properties.Settings.Default.PathToDFExe == "" || !File.Exists(Properties.Settings.Default.PathToDFExe))
            {
                MessageBox.Show("Dwarf Fortress.exe not found. Please locate.");
                this.LocateDF2010(sender, e);
            }

            FileInfo fi = new FileInfo(Properties.Settings.Default.PathToDFExe);
            string saveDir = String.Format("{0}\\data\\save", fi.DirectoryName);
            ((App)Application.Current).saveDirectory = new DFSaveDirectory(saveDir);
            this.PopulateSaveGames(sender, e);
        }

        private void PopulateSaveGames(object sender, RoutedEventArgs e)
        {
            DFSaveDirectory saveDir = (DFSaveDirectory)((App)Application.Current).saveDirectory;
            foreach (string saveGame in saveDir.ListAllSaves())
                this.comboSaveSelect.Items.Add(saveGame);
        }
    }
}
