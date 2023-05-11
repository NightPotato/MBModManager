using MahApps.Metro.Controls.Dialogs;
using System.Diagnostics;
using System.IO;

namespace MBModManager.Events {
    internal class GeneralEvents {

        public static void OPEN_WEB(string url) {
            if (string.IsNullOrEmpty(url)) { return; }
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }


        public static bool CHECK_DEPS() {
            // Check if all mods loaded Depends_on is met else return false
            // CALL GAME_LAUNCH_MISSINGMODS PASSING MISSING MODS.
            return true;
        }


        public static async void GAME_LAUNCH_FAILED(MainWindow mv) {
            MetroDialogSettings dialogSettings = new MetroDialogSettings();
            dialogSettings.ColorScheme = MetroDialogColorScheme.Inverted;
            await mv.ShowMessageAsync("Game Launch Failed!", "Something went wrong on our end. Please report this issue on github.", MessageDialogStyle.Affirmative, dialogSettings);
        }

        public static async void GAME_LAUNCH_MISSINGMODS(MainWindow mv, string[] missingMods) {
            MetroDialogSettings dialogSettings = new MetroDialogSettings();
            dialogSettings.ColorScheme = MetroDialogColorScheme.Inverted;

            // Format Missing Mods list
            string missingMods_output = "[]";

            await mv.ShowMessageAsync("Missing Required Mods", missingMods_output, MessageDialogStyle.Affirmative, dialogSettings);
        } 

        public static void CLEANUP_WORKDIR() {
            string workDir = System.AppDomain.CurrentDomain.BaseDirectory + "\\workdir";
            if (Directory.Exists(workDir)) {
                Directory.Delete(workDir, true);
            }
        }

    }
}
