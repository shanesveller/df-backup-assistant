using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DFBackupAssistant.Classes
{
    public class DFSettings
    {
        public enum AutoSaveType { None, Yearly, Seasonal };
        
        public AutoSaveType SaveType { get; set; }
        public bool AutoBackup { get; set; }
        public bool CompressSaves { get; set; }
        public bool InitialSave { get; set; }
        public bool PauseOnAutosave { get; set; }
        public bool PauseOnLoad { get; set; }

        private FileInfo InitTxt { get; set; }
        private FileInfo DInitTxt { get; set; }

        public DFSettings(string path)
        {
            FileInfo exe = new FileInfo(path);
            this.InitTxt = new FileInfo(Path.Combine(exe.DirectoryName, "data", "init", "init.txt"));
            this.DInitTxt = new FileInfo(Path.Combine(exe.DirectoryName, "data", "init", "d_init.txt"));
            if (!InitTxt.Exists || DInitTxt.Exists)
                throw new NullReferenceException("init.txt or d_init.txt missing.");
        }
    }
}
