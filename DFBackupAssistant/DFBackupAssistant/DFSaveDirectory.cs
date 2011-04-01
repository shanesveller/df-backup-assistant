using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DFBackupAssistant
{
    public class DFSaveDirectory
    {
        public string FullPath { get; set; }
        public List<DFSave> SaveGames { get; set; }

        public DFSaveDirectory(string fp)
        {
            this.FullPath = fp;
            this.SaveGames = new List<DFSave>();
            DirectoryInfo[] dirs = new DirectoryInfo(this.FullPath).GetDirectories();
            foreach (DirectoryInfo subDir in dirs)
                this.SaveGames.Add(new DFSave(this.FullPath, subDir.Name));
        }
    }
}
