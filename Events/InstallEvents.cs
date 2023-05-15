using MahApps.Metro.Controls.Dialogs;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MBModManager.Events;

internal static class InstallEvents {

    public static async void InstallBepInEx(MainWindow mv) {
        // Dialog Controller Setup
        var dialogSettings = new MetroDialogSettings
        {
            ColorScheme = MetroDialogColorScheme.Inverted
        };
        var controller = await mv.ShowProgressAsync("Please wait...", "Downloading BepInEx from Github Releases.", false, dialogSettings);
        controller.SetProgressBarForegroundBrush(new SolidColorBrush(Color.FromRgb(71, 125, 17)));


        // Setup WorkDir for Operations
        var workDir = System.AppDomain.CurrentDomain.BaseDirectory + "\\workDir";
        var worDirExists = Directory.Exists(workDir);

        if (!worDirExists) {
            Directory.CreateDirectory(workDir);
        }

        // Verify Internal Data Exists
        if (mv.ClientSettings.InternalData.BepInExUrl == null) {
            Handlers.ErrorHandler.BepInExInstallFailed(controller, "Some of the applications internal data is missing, please delete settings.json and relaunch MBModManager.");
            return;
        }

        // Check if the GamePath is set in the Options.
        if (string.IsNullOrWhiteSpace(mv.ClientSettings.GamePath)) {
            Handlers.ErrorHandler.BepInExInstallFailed(controller, "The Game Path was not set in the options. Please set a Game Path before installing a BepInEx.");
            return;
        }

        // GetLatestRelease from Github
        await Task.Delay(2000);
        var zipPath = workDir + "BepInEx.zip";
        using (var client = new HttpClient()) {
            controller.SetProgress(0.25f);
            var response = await client.GetByteArrayAsync(mv.ClientSettings.InternalData.BepInExUrl);
            await File.WriteAllBytesAsync(zipPath, response);
            controller.SetProgress(0.35f);
            controller.SetMessage("Finished Downloading BepInEx.");
        }

        // Unzip Release in GamePath
        await Task.Delay(2000);
        controller.SetMessage("Installing BepInEx to Game Directory.");
        controller.SetProgress(0.45f);

        try {
            ZipFile.ExtractToDirectory(zipPath, mv.ClientSettings.GamePath);
        } catch {
            Handlers.ErrorHandler.BepInExInstallFailed(controller, "Game already contains some of BepInEx files. Please remove these files from your game before installing.");
            return;
        }

        // Delete BepInEx.zip from AppDirectory
        await Task.Delay(2000);
        controller.SetMessage("Cleaning up install files.");
        File.Delete(zipPath);
        controller.SetProgress(0.65f);


        // Close Install Progress.
        await Task.Delay(2000);

        controller.SetProgress(0.99999999999f);
        controller.SetTitle("Success!");
        controller.SetMessage("We successfully installed BepInEx. Please start the game and finish generating all of BepInEx folders, then you can freely install mods in your game.");
        await Task.Delay(5000);
        await controller.CloseAsync();
    }


    public static async void InstallMod(MainWindow mv) {
        var dialogSettings = new MetroDialogSettings
        {
            ColorScheme = MetroDialogColorScheme.Inverted
        };
        var controller = await mv.ShowProgressAsync("Installing Mod!", "Moving mod.zip to working-directory.", false, dialogSettings);
        controller.SetProgressBarForegroundBrush(new SolidColorBrush(Color.FromRgb(71, 125, 17)));

        await Task.Delay(5000);
        await controller.CloseAsync();
    }
}