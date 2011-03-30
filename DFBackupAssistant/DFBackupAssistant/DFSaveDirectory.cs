using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DFBackupAssistant
{
    public class DFSaveDirectory
    {
        public string fullPath { get; set; }
        public DFSaveDirectory(string fp)
        {
            this.fullPath = fp;
        }

        public List<string> ListAllSaves()
        {
            List<string> subDirs = new List<string>();

            DirectoryInfo[] dirs = new DirectoryInfo(this.fullPath).GetDirectories();
            var query = from d in dirs
                        where (d.Name != "current")
                        select d.Name;
            foreach (string subDir in query)
                subDirs.Add(subDir);

            return subDirs;
        }
    }
}
