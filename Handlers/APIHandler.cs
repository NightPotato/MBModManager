using System;
using MBModManager.Data;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

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

        foreach (var info in obj) {
            if (mv.InstalledMods.Contains(info)) {
                return;
            }
            mv.ModList.Add(info);
        }
    }

    public static async Task<ModInfo> GetModById(MainWindow mv, string id)
    {
        using var httpClient = new HttpClient();
        var json = await httpClient.GetStringAsync(mv.ClientSettings.InternalData.BaseApiUrl + "/" + id);
        var obj = JsonConvert.DeserializeObject<ModInfo>(json);

        return obj ?? new ModInfo(1, "Failed to Get Mod List", "If you see this message, please try again later.", "", "PotatDev180#1911", 1, Array.Empty<Tag>());
    }

    public static void RefreshModList(MainWindow mv) {
        mv.ModList.Clear();
        GetAllMods(mv);
    }
}