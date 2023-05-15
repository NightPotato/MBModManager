using MBModManager.Data;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace MBModManager.Handlers
{
    internal class DataHandler
    {

        public static Settings LoadAppSettings()
        {

            string confPath = AppDomain.CurrentDomain.BaseDirectory + "/settings.json";
            if (File.Exists(confPath))
            {
                Settings appSettings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(confPath));
                if (appSettings == null)
                {
                    appSettings = new Settings(GetGamePathFromRegistry(), "No Path Set", "0.0.0");

                }
                return appSettings;
            }
            else
            {
                // Create & Save New Settings to File.
                Settings appSettings = new Settings(GetGamePathFromRegistry(), "No Path Set", "0.0.0");
                File.WriteAllText(confPath, JsonConvert.SerializeObject(appSettings, Formatting.Indented));
                return appSettings;
            }

        }

        public static void SaveAppSettings(Settings appSettings)
        {
            string confPath = System.AppDomain.CurrentDomain.BaseDirectory + "/settings.json";
            File.WriteAllText(confPath, JsonConvert.SerializeObject(appSettings, Formatting.Indented));
        }


        public static string GetGamePathFromRegistry() {
            using (RegistryKey registryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64)) {
                RegistryKey registryKey2;
                try {
                    registryKey2 = registryKey.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\Steam App 1520370");
                } catch (Exception ex) {
                    return "";
                }
                object value = registryKey2.GetValue("InstallLocation");
                if (!string.IsNullOrWhiteSpace(value.ToString())) {
                    return value.ToString();
                }

                return "";
            }
        }


        public static ObservableCollection<ModInfo> GetModList() {

            var modList = new ObservableCollection<ModInfo>();
            for (int i = 0; i < 50; i++) {
                modList.Add(new ModInfo(1, "Failed to Get Mod List", "If you see this message, please try again later.", "", "PotatDev180#1911", 1, new Tag[] { }));
            }


            return modList;
        }

    }
}
