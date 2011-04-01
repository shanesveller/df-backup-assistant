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
    public class DFSave
    {
        public string ParentDir { get; set; }
        public string Name { get; set; }
        public string FullPath { get { return Path.Combine(this.ParentDir, this.Name); } }

        public DFSave(string dir, string n)
        {
            this.ParentDir = dir;
            this.Name = n;
        }

        public override string ToString()
        {
            return this.Name;
        }

        public void Archive()
        {
            using (var zip = new ZipFile())
            {
                zip.CompressionLevel = CompressionLevel.None;
                zip.AddDirectory(this.FullPath, this.Name);
                zip.Save(Path.Combine(this.ParentDir, this.Name + ".zip"));
            }
            System.Diagnostics.Process.Start(this.ParentDir);
        }

        public void Archive(bool deleteAfter)
        {
            this.Archive();
            if (deleteAfter)
                this.Delete();
        }

        public void Delete()
        {
            new DirectoryInfo(this.FullPath).Delete(true);
        }
    }
}
