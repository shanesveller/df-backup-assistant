using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using Ionic.Zip;

namespace DFBackupAssistant
{
    public class Backup
    {
        public string Filename { get; set; }
        public string Name { get; set; }
        public Backup(string f, string n)
        {
            this.Filename = f;
            this.Name = n;
        }
        public void Restore(string folder, bool overwrite = false)
        {
            using (ZipFile zip = new ZipFile(this.Filename))
            {
                if (Directory.Exists(folder))
                {
                    if (overwrite)
                        Directory.Delete(folder, true);
                    else
                        throw new InvalidOperationException("Invalid operation: save directory exists");
                }
                zip.ExtractAll(folder);
            }
        }
        public override string ToString()
        {
            return this.Name;
        }
    }
}
