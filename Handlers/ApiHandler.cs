using System;
using System.Threading.Tasks;
using MBModManager.Models;

namespace MBModManager.Handlers;

internal static class ApiHandler
{
    public static ModInfo[] GetAllMods()
    {
        var response = WebHandler.GetJson<ModInfo[]>(Constants.ModsUrl).Result;
        return response ?? Array.Empty<ModInfo>();
    }

    public static async Task<ModInfo> GetModById(string id)
    {
        var response = await WebHandler.GetJson<ModInfo>(Constants.ModsUrl + "/" + id);
        return response ?? new ModInfo(1, "Failed to Get Mod List", "If you see this message, please try again later.", "", "PotatDev180#1911", 1, Array.Empty<Tag>());
    }
}
