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
        

        public Settings(string game, string saves, string ver) {
            GamePath = game;
            SavesPath = saves;
            GameVersion = ver;
            string[] internalData = { "v1.0.0", "4f78160a-6f22-4d53-94ce-afd993ffd6e5", "https://github.com/BepInEx/BepInEx/releases/download/v5.4.21/BepInEx_x64_5.4.21.0.zip" };
            InternalData = internalData;
        }
    }

    public class ModInfo {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Version { get; set; }
        public string? Author { get; set; }
        public string[]? ModFiles { get; set; }
        
    }

}
