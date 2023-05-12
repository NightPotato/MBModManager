using MBModManager.Data;
using Newtonsoft.Json;
using System.IO;

namespace MBModManager.Handlers
{
    internal class DataHandler
    {

        public static Settings LoadAppSettings()
        {

            string confPath = System.AppDomain.CurrentDomain.BaseDirectory + "/settings.json";
            if (File.Exists(confPath))
            {
                Settings appSettings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(confPath));
                if (appSettings == null)
                {
                    appSettings = new Settings("No Path Set", "No Path Set", "0.0.0");
                }
                return appSettings;
            }
            else
            {
                // Create & Save New Settings to File.
                Settings appSettings = new Settings("No Path Set", "No Path Set", "0.0.0");
                File.WriteAllText(confPath, JsonConvert.SerializeObject(appSettings, Formatting.Indented));
                return appSettings;
            }

        }

        public static void SaveAppSettings(Settings appSettings)
        {
            string confPath = System.AppDomain.CurrentDomain.BaseDirectory + "/settings.json";
            File.WriteAllText(confPath, JsonConvert.SerializeObject(appSettings, Formatting.Indented));
        }


    }
}
