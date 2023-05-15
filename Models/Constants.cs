using System;

namespace MBModManager.Models;

public static class Constants
{
    public const string Version = "v1.0.0";
    public const string Uuid = "4f78160a-6f22-4d53-94ce-afd993ffd6e5";
    public const string BepInExUrl = "https://github.com/BepInEx/BepInEx/releases/download/v5.4.21/BepInEx_x64_5.4.21.0.zip";
    public const string ModsUrl = "https://mods-monbazou-api.amenofisch.dev/api/mods";

    public const string MonBazouRegistry = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\Steam App 1520370";
    public static string dataPath = AppDomain.CurrentDomain.BaseDirectory + "/data.json";
}
