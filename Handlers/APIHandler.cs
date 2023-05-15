using MahApps.Metro.Controls.Dialogs;
using MBModManager.Data;
using Newtonsoft.Json;
using System;
using System.Net.Http;

namespace MBModManager.Handlers {
    internal class APIHandler {

        public async static void GetAllMods(MainWindow mv) {
            using (var httpClient = new HttpClient()) {
                var json = await httpClient.GetStringAsync(mv.clientSettings.InternalData.BaseAPIURL);
                var obj = JsonConvert.DeserializeObject<ModInfo[]>(json);

                if (obj == null) {
                    mv.ModList.Add(new ModInfo(1, "Failed to Get Mod List", "If you see this message, please try again later.", "", "PotatDev180#1911", 1, new Tag[] { }));
                    return;
                }

                foreach (ModInfo info in obj) {
                    mv.ModList.Add(info);
                }
            }
        }

        public static void RefreshModList(MainWindow mv) {
            mv.ModList.Clear();
            GetAllMods(mv);
        }

    }
}
