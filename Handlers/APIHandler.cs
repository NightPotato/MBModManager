using System;
using System.Threading.Tasks;
using MBModManager.Models;

namespace MBModManager.Handlers;

internal static class ApiHandler {

    public static async void GetAllMods(MainWindow mv) {
        var response = await WebHandler.GetJson<ModInfo[]>(InternalData.ModsUrl);
        if (response == null) {
            mv.ModList.Add(new ModInfo(1, "Failed to Get Mod List", "If you see this message, please try again later.", "", "PotatDev180#1911", 1, Array.Empty<Tag>()));
            return;
        }

        foreach (var info in response) {
            if (mv.InstalledMods.Contains(info)) return;
            mv.ModList.Add(info);
        }
    }

    public static async Task<ModInfo> GetModById(string id)
    {
        var response = await WebHandler.GetJson<ModInfo>(InternalData.ModsUrl + "/" + id);
        return response ?? new ModInfo(1, "Failed to Get Mod List", "If you see this message, please try again later.", "", "PotatDev180#1911", 1, Array.Empty<Tag>());
    }

    public static void RefreshModList(MainWindow mv) {
        mv.ModList.Clear();
        GetAllMods(mv);
    }
}