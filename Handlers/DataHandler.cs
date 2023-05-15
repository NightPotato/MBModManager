using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using MBModManager.Models;

namespace MBModManager.Handlers;

internal static class DataHandler
{
    public static Settings LoadAppSettings()
    {
        var confPath = AppDomain.CurrentDomain.BaseDirectory + "/settings.json";
        if (File.Exists(confPath))
        {
            var appSettings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(confPath));
            if (appSettings == null)
            {
                appSettings = new Settings(GetGamePathFromRegistry(), "No Path Set", "0.0.0");
            }
            return appSettings;
        }
        else
        {
            // Create & Save New Settings to File.
            var appSettings = new Settings(GetGamePathFromRegistry(), "No Path Set", "0.0.0");
            File.WriteAllText(confPath, JsonConvert.SerializeObject(appSettings, Formatting.Indented));
            return appSettings;
        }
    }

    public static void SaveAppSettings(Settings appSettings)
    {
        var confPath = AppDomain.CurrentDomain.BaseDirectory + "/settings.json";
        File.WriteAllText(confPath, JsonConvert.SerializeObject(appSettings, Formatting.Indented));
    }

    private static string GetGamePathFromRegistry()
    {
        using var registryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
        var registryKey2 = registryKey.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\Steam App 1520370");

        if (registryKey2?.GetValue("InstallLocation") is not string installLocation) {
            return string.Empty;
        }

        return !string.IsNullOrWhiteSpace(installLocation) ? installLocation : string.Empty;
    }

    public static void SaveInstalledMods(List<ModInfo> mods) {
        //  Saving to data.json
        var confPath = AppDomain.CurrentDomain.BaseDirectory + "/data.json";
        File.WriteAllText(confPath, JsonConvert.SerializeObject(mods, Formatting.Indented));
    }

    public static List<ModInfo> LoadInstalledMods() {
        // Loading from data.json
        var dataPath = AppDomain.CurrentDomain.BaseDirectory + "/data.json";
        if (!File.Exists(dataPath)) return new List<ModInfo>();
        var appSettings = JsonConvert.DeserializeObject<List<ModInfo>>(File.ReadAllText(dataPath));
        return appSettings ?? new List<ModInfo>();
    }

    public static ObservableCollection<ModInfo> GetModList() {

        var modList = new ObservableCollection<ModInfo>();
        for (var i = 0; i < 50; i++) {
            modList.Add(new ModInfo(1, "Failed to Get Mod List", "If you see this message, please try again later.", "", "PotatDev180#1911", 1, Array.Empty<Tag>()));
        }

        return modList;
    }
}
