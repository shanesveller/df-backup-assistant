using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Ionic.Zip;
using Ionic.Zlib;

namespace DFBackupAssistant
{
    public class Save
    {
        public string ParentDir { get; set; }
        public string Name { get; set; }
        public string FullPath { get { return Path.Combine(this.ParentDir, this.Name); } }

        public Save(string dir, string n)
        {
            this.ParentDir = dir;
            this.Name = n;
        }

        public override string ToString()
        {
            return this.Name;
        }

        public void Archive(BackupDirectory targetDir, bool deleteAfter = false, bool timestampArchive = true)
        {
            if (new FileInfo(this.ArchiveFilename(targetDir, timestampArchive)).Exists)
                throw new InvalidOperationException("Destination filename exists.");
                            
            using (var zip = new ZipFile())
            {
                zip.CompressionLevel = CompressionLevel.None;
                zip.AddDirectory(this.FullPath);
                zip.Save(this.ArchiveFilename(targetDir, timestampArchive));
            }

            if (deleteAfter)
                this.Delete();
        }

        public string ArchiveFilename(BackupDirectory targetDir, bool timestamped)
        {
            if (timestamped)
                return String.Format("{0}.{1}.zip", Path.Combine(targetDir.FullPath, this.Name), DateTime.Now.ToString("yyyyMMdd-HHmm"));
            else
                return String.Format("{0}.zip", Path.Combine(targetDir.FullPath, this.Name));
        }

        public void Delete()
        {
            new DirectoryInfo(this.FullPath).Delete(true);
        }
    }
}
