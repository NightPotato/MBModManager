using MahApps.Metro.Controls.Dialogs;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MBModManager.Events {
    internal class InstallEvents {

        public static async void BIX_INSTALL(MainWindow mv) {
            MetroDialogSettings dialogSettings = new MetroDialogSettings();
            dialogSettings.ColorScheme = MetroDialogColorScheme.Inverted;
            var controller = await mv.ShowProgressAsync("Please wait...", "Downloading BepInEx from Github Releases.", false, dialogSettings);
            controller.SetProgressBarForegroundBrush(new System.Windows.Media.SolidColorBrush(Color.FromRgb(71, 125, 17)));
            string zipPath = System.AppDomain.CurrentDomain.BaseDirectory + "BepInEx.zip";

            if (mv.clientSettings.InternalData == null || mv.clientSettings.InternalData[2] == null) {
                Handlers.ErrorHandler.BIX_INSTALL_FAILED(controller, "Some of the applications internal data is missing, please delete settings.json and relaunch MBModManager.");
                return;
            }

            // Check if the GamePath is set in the Options.
            if (mv.clientSettings.GamePath == null || mv.clientSettings.GamePath == "") {
                Handlers.ErrorHandler.BIX_INSTALL_FAILED(controller, "The Game Path was not set in the options. Please set a Game Path before installing a BepInEx.");
                return;
            }

            // GetLatestRelease from Github
            await Task.Delay(2000);
            using (var client = new HttpClient()) {
                controller.SetProgress(0.25f);
                var response = await client.GetByteArrayAsync(mv.clientSettings.InternalData[2]);
                File.WriteAllBytes(zipPath, response);
                controller.SetProgress(0.35f);
                controller.SetMessage("Finished Downloading BepInEx.");
            }

            // Unzip Release in GamePath
            await Task.Delay(2000);
            controller.SetMessage("Installing BepInEx to Game Directory.");
            controller.SetProgress(0.45f);

            try {
                ZipFile.ExtractToDirectory(zipPath, mv.clientSettings.GamePath);
            } catch {
                Handlers.ErrorHandler.BIX_INSTALL_FAILED(controller, "Game already contains some of BepInEx files. Please remove these files from your game before installing.");
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


    }
}
