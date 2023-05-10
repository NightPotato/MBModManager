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
        public string? Author { get; set; }
        public string[]? Tags { get; set; }
        public string[]? depends_on { get; set; }
        public string[]? ModFiles { get; set; }


        public ModInfo(string modName, string desc, string auth) {
            Name = modName;
            Description = desc;
            Author = auth;
            Tags = new string[] { }; 
            depends_on = new string[] { };
            ModFiles = new string[] { };
        }
    }

}
