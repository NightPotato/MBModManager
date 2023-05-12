namespace MBModManager.Data
{
    public class Settings
    {


        public string? GamePath { get; set; }
        public string? GameVersion { get; set; }
        public string? SavesPath { get; set; }
        public InternalData InternalData { get; set; }

        public Settings(string game, string saves, string ver)
        {
            GamePath = game;
            SavesPath = saves;
            GameVersion = ver;
            InternalData = new InternalData("v1.0.0", "4f78160a-6f22-4d53-94ce-afd993ffd6e5", "https://github.com/BepInEx/BepInEx/releases/download/v5.4.21/BepInEx_x64_5.4.21.0.zip", "https://mods-monbazou-api.amenofisch.dev/api/mods");
        }
    }

    public class InternalData
    {
        public string? AppVersion { get; set; }
        public string? AppUID { get; set; }
        public string? BepInExURL { get; set; }
        public string? BaseAPIURL { get; set; }

        public InternalData(string? appVersion, string? appUID, string? bepInExURL, string? baseAPIUrl)
        {
            AppVersion = appVersion;
            AppUID = appUID;
            BepInExURL = bepInExURL;
            BaseAPIURL = baseAPIUrl;
        }
    }

}
