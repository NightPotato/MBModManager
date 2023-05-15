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
        File.WriteAllText(Constants.dataPath, JsonConvert.SerializeObject(appSettings, Formatting.Indented));
    }

    private static string GetGamePathFromRegistry()
    {
        using var registryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
        var registryKey2 = registryKey.OpenSubKey(Constants.MonBazouRegistry);

        if (registryKey2?.GetValue("InstallLocation") is not string installLocation) {
            return string.Empty;
        }

        return !string.IsNullOrWhiteSpace(installLocation) ? installLocation : string.Empty;
    }

    public static void SaveInstalledMods(List<ModInfo> mods) {
        //  Saving to data.json
        File.WriteAllText(Constants.dataPath, JsonConvert.SerializeObject(mods, Formatting.Indented));
    }

    public static List<ModInfo> LoadInstalledMods() {
        // Loading from data.json
        if (!File.Exists(Constants.dataPath)) return new List<ModInfo>();
        var appSettings = JsonConvert.DeserializeObject<List<ModInfo>>(File.ReadAllText(Constants.dataPath));
        return appSettings ?? new List<ModInfo>();
    }
}
