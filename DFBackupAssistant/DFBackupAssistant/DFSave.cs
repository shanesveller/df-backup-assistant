using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace DFBackupAssistant
{
    public class DFSave
    {
        public string ParentDir { get; set; }
        public string Name { get; set; }
        public string FullPath { get { return String.Format(@"{0}\{1}", this.ParentDir, this.Name); } }

        public DFSave(string dir, string n)
        {
            this.ParentDir = dir;
            this.Name = n;
        }

        public override string ToString()
        {
            return this.Name;
        }

        public void Archive(string outputDir, string outputFilename)
        {
            var startInfo = new ProcessStartInfo(Properties.Settings.Default.PathTo7zaExe);
            startInfo.UseShellExecute = false;
            startInfo.WorkingDirectory = this.ParentDir;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.Arguments = String.Format("a \"{0}\" \"{1}\\\" -t7z -ssw -mx0", Path.Combine(outputDir, outputFilename), this.Name);
            Process proc = Process.Start(startInfo);
            proc.WaitForExit();
            int exitCode = proc.ExitCode;
            switch (exitCode)
            {
                case 0:
                    break;
                case 1:
                    // Warning
                case 2:
                    // Fatal Error
                case 7:
                    // Command Line Error
                case 8:
                    // Not Enough Memory Error
                case 255:
                    // User stopped the process
                default:
                    // Unknown
                    break;
                    
            }
            System.Diagnostics.Process.Start(outputDir);

            /*
            Directory.SetCurrentDirectory(this.ParentDir);
            SevenZipCompressor compressor = new SevenZipCompressor();
            compressor.FastCompression = true;
            compressor.DirectoryStructure = true;
            compressor.PreserveDirectoryRoot = false;
            compressor.CompressionLevel = CompressionLevel.None;
            compressor.ArchiveFormat = OutArchiveFormat.SevenZip;
            compressor.CompressDirectory(this.Name, outputDir + "\\" + outputFilename);
            System.Diagnostics.Process.Start(outputDir);
             */
        }

        public void Archive(string outputDir, string outputFilename, bool deleteAfter)
        {
            this.Archive(outputDir, outputFilename);
            if (deleteAfter)
                this.Delete();
        }

        public void Delete()
        {
            new DirectoryInfo(this.FullPath).Delete(true);
        }
    }
}
