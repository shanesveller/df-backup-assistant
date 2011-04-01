using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DFBackupAssistant
{
    public class BackupDirectory
    {
        public string FullPath { get; set; }
        public List<Backup> Backups { get; set; }

        public BackupDirectory(string f)
        {
            this.FullPath = f;
            this.Backups = new List<Backup>();
            FileInfo[] backupFiles = new DirectoryInfo(f).GetFiles("*.zip");
            foreach (FileInfo file in backupFiles)
                this.Backups.Add(new Backup(file.FullName, file.Name));
        }
    }
}
