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

        public SaveDirectory(string fp)
        {
            this.FullPath = fp;
            this.SaveGames = new List<Save>();
            this.RefreshSaveGames();
        }

        public void RefreshSaveGames()
        {
            this.SaveGames.Clear();
            DirectoryInfo[] dirs = new DirectoryInfo(this.FullPath).GetDirectories();
            foreach (DirectoryInfo subDir in dirs)
                this.SaveGames.Add(new Save(this.FullPath, subDir.Name));
        }
    }
}
