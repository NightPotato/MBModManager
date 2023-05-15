using MahApps.Metro.Controls.Dialogs;
using System.Diagnostics;
using System.IO;

namespace MBModManager.Events;

internal static class GeneralEvents {

    public static void OpenWeb(string url) {
        if (string.IsNullOrWhiteSpace(url)) { return; }
        Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
    }


    public static bool CheckDependencies() {
        // Check if all mods loaded Depends_on is met else return false
        // CALL GAME_LAUNCH_MISSINGMODS PASSING MISSING MODS.
        return true;
    }


    public static async void GameLaunchFailed(MainWindow mv) {
        var dialogSettings = new MetroDialogSettings
        {
            ColorScheme = MetroDialogColorScheme.Inverted
        };
        await mv.ShowMessageAsync("Game Launch Failed!", "Something went wrong on our end. Please report this issue on github.", MessageDialogStyle.Affirmative, dialogSettings);
    }

    public static async void GameLaunchMissingMods(MainWindow mv, string[] missingMods) {
        var dialogSettings = new MetroDialogSettings
        {
            ColorScheme = MetroDialogColorScheme.Inverted
        };

        // Format Missing Mods list
        const string missingModsOutput = "[]";

        await mv.ShowMessageAsync("Missing Required Mods", missingModsOutput, MessageDialogStyle.Affirmative, dialogSettings);
    }

    public static void CleanupWorkDir() {
        var workDir = System.AppDomain.CurrentDomain.BaseDirectory + "\\workdir";
        if (Directory.Exists(workDir)) {
            Directory.Delete(workDir, true);
        }
    }
}