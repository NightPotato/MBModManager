using Newtonsoft.Json;
using System.IO;

namespace MBModManager
{
    internal class DataManager
    {

        public static Settings LoadAppSettings()
        {

            string confPath = System.AppDomain.CurrentDomain.BaseDirectory + "/settings.json";
            if (File.Exists(confPath)) {
                Settings appSettings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(confPath));
                return appSettings;
            } else {

                // Create Basic Config Here.
                Settings appSettings = new Settings();
                appSettings.GamePath = "No Path Set";
                appSettings.SavesPath = "No Path Set";
                appSettings.GameVersion = "0.0.0";

                string[] internalData = { "v1.0.0", "4f78160a-6f22-4d53-94ce-afd993ffd6e5", "https://github.com/BepInEx/BepInEx/releases/download/v5.4.21/BepInEx_x64_5.4.21.0.zip" };

                appSettings.InternalData = internalData;

                // Save New Settings to File.
                File.WriteAllText(confPath, JsonConvert.SerializeObject(appSettings, Formatting.Indented));
                return appSettings;
            }

        }

        public static void SaveAppSettings(Settings appSettings) {
            string confPath = System.AppDomain.CurrentDomain.BaseDirectory + "/settings.json";
            File.WriteAllText(confPath, JsonConvert.SerializeObject(appSettings, Formatting.Indented));
        }


    }
}
