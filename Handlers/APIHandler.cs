using System;
using MBModManager.Data;
using Newtonsoft.Json;
using System.Net.Http;

namespace MBModManager.Handlers;

internal static class ApiHandler {

    public static async void GetAllMods(MainWindow mv)
    {
        using var httpClient = new HttpClient();
        var json = await httpClient.GetStringAsync(mv.ClientSettings.InternalData.BaseApiUrl);
        var obj = JsonConvert.DeserializeObject<ModInfo[]>(json);

        if (obj == null) {
            mv.ModList.Add(new ModInfo(1, "Failed to Get Mod List", "If you see this message, please try again later.", "", "PotatDev180#1911", 1, Array.Empty<Tag>()));
            return;
        }

        foreach (var modInfo in obj) {
            mv.ModList.Add(modInfo);
        }
    }

    public static void RefreshModList(MainWindow mv) {
        mv.ModList.Clear();
        GetAllMods(mv);
    }
}