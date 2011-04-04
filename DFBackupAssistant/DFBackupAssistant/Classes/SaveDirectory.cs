using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DFBackupAssistant
{
    public class SaveDirectory
    {
        public string FullPath { get; set; }
        public List<Save> SaveGames { get; set; }
        public DirectoryInfo DirectoryInfo { get { return new DirectoryInfo(this.FullPath); } }
        private FileSystemWatcher Watcher { get; set; }
        public bool Watching {
            get { return this.Watcher.EnableRaisingEvents; }
            set { this.Watcher.EnableRaisingEvents = value; }
        }

        public SaveDirectory(string fp)
        {
            this.FullPath = fp;
            this.SaveGames = new List<Save>();
            this.RefreshSaveGames();

            this.Watcher = new FileSystemWatcher(fp, "world.sav");
            this.Watcher.IncludeSubdirectories = true;
            this.Watcher.Changed += this.watcherChanged;
            this.Watcher.Created += this.watcherCreated;
            this.Watcher.Deleted += this.watcherDeleted;
            this.Watcher.EnableRaisingEvents = true;
        }

        public void RefreshSaveGames()
        {
            this.SaveGames.Clear();
            DirectoryInfo[] dirs = new DirectoryInfo(this.FullPath).GetDirectories();
            foreach (DirectoryInfo subDir in dirs)
                this.SaveGames.Add(new Save(this.FullPath, subDir.Name));
        }

        private void watcherChanged(object sender, System.IO.FileSystemEventArgs e)
        {
            System.Windows.MessageBox.Show(e.ChangeType + ": " + e.FullPath);
            this.RefreshSaveGames();
        }

        private void watcherCreated(object sender, System.IO.FileSystemEventArgs e)
        {
            System.Windows.MessageBox.Show(e.ChangeType + ": " + e.FullPath);
            this.RefreshSaveGames();
        }

        private void watcherDeleted(object sender, System.IO.FileSystemEventArgs e)
        {
            System.Windows.MessageBox.Show(e.ChangeType + ": " + e.FullPath);
            this.RefreshSaveGames();
        }
    }
}
