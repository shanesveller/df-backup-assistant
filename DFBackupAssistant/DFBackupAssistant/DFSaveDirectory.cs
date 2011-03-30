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
            var query = from d in dirs
                        where (d.Name != "current")
                        select d.Name;
            foreach (string subDir in query)
                this.SaveGames.Add(new DFSave(this.FullPath, subDir));
        }
    }
}
