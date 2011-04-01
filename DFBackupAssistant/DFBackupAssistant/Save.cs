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

        public void Archive(bool deleteAfter = false, bool timestampArchive = true)
        {
            using (var zip = new ZipFile())
            {
                zip.CompressionLevel = CompressionLevel.None;
                zip.AddDirectory(this.FullPath, this.Name);
                string filename;

                if (timestampArchive)
                    filename = String.Format("{0}.{1}.zip", Path.Combine(this.ParentDir, this.Name), DateTime.Now.ToString("yyyyMMdd-HHmm"));
                else
                    filename = String.Format("{0}.zip", Path.Combine(this.ParentDir, this.Name));
                
                zip.Save(filename);
            }

            if (deleteAfter)
                this.Delete();

            System.Diagnostics.Process.Start(this.ParentDir);
        }

        public void Delete()
        {
            new DirectoryInfo(this.FullPath).Delete(true);
        }
    }
}
