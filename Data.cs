using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBModManager
{
    public class Settings {
        public string? GamePath { get; set; }
        public string? GameVersion { get; set; }
        public string? SavesPath { get; set; }

        public string[]? InternalData { get; set; }
    }

    public class ModInfo {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Version { get; set; }
        public string? Author { get; set; }
        public string[]? ModFiles { get; set; }
        
    }

}
