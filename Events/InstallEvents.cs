using MahApps.Metro.Controls.Dialogs;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Media;
using MBModManager.Models;
using MBModManager.Handlers;
using System.Text.RegularExpressions;
using System.Windows;

namespace MBModManager.Events;

internal static class InstallEvents {

    public static async void InstallBepInEx(MainWindow mv) {
        // Dialog Controller Setup
        var dialogSettings = new MetroDialogSettings();
        var controller = await mv.ShowProgressAsync("Please wait...", "Downloading BepInEx from Github Releases.", false, dialogSettings);
        controller.SetProgressBarForegroundBrush(new SolidColorBrush(Color.FromRgb(71, 125, 17)));

        // Setup WorkDir for operations
        var workDir = System.AppDomain.CurrentDomain.BaseDirectory + "\\workDir";
        var worDirExists = Directory.Exists(workDir);

        if (!worDirExists) {
            Directory.CreateDirectory(workDir);
        }

        // Check if the GamePath is set in the Options.
        if (string.IsNullOrEmpty(mv.ClientSettings.GamePath)) {
            ErrorHandler.BepInExInstallFailed(controller, "The Game Path was not set in the options. Please set a Game Path before installing a BepInEx.");
            return;
        }

        // GetLatestRelease from Github
        var zipPath = workDir + "BepInEx.zip";
        using (var client = new HttpClient()) {
            controller.SetProgress(0.25f);
            var response = await client.GetByteArrayAsync(InternalData.BepInExUrl);
            File.WriteAllBytes(zipPath, response);
            controller.SetProgress(0.35f);
            controller.SetMessage("Finished Downloading BepInEx.");
        }

        // Unzip Release in GamePath
        controller.SetMessage("Installing BepInEx to Game Directory.");
        controller.SetProgress(0.45f);

        try {
            ZipFile.ExtractToDirectory(zipPath, mv.ClientSettings.GamePath);
        } catch {
            ErrorHandler.BepInExInstallFailed(controller, "Game already contains some of BepInEx files. Please remove these files from your game before installing.");
            return;
        }

        // Delete BepInEx.zip from AppDirectory
        controller.SetMessage("Cleaning up install files.");
        File.Delete(zipPath);
        controller.SetProgress(0.65f);

        // Close Install Progress.
        controller.SetProgress(0.99999999999f);
        controller.SetTitle("Success!");
        controller.SetMessage("We successfully installed BepInEx. Please start the game and finish generating all of BepInEx folders, then you can freely install mods in your game.");
        await Task.Delay(5000);
        await controller.CloseAsync();
    }


    public static async void InstallMod(MainWindow mv, DragEventArgs e) {
        // Setup Dialog Controller
        var dialogSettings = new MetroDialogSettings();
        var controller = await mv.ShowProgressAsync("Installing Mod!", "Moving Mod to working-directory.", false, dialogSettings);
        controller.SetProgressBarForegroundBrush(new SolidColorBrush(Color.FromRgb(71, 125, 17)));

        // Check if GamePath is set
        if (mv.ClientSettings.GamePath == null || mv.ClientSettings.GamePath == "Path Not Set") {
            ErrorHandler.ModInstallFailed(controller, "GamePath was not set in options, please set the GamePath and try again.", false);
            return;
        }

        // Check if Multiple Files were dropped in the install.
        var filePaths = (string[]?)e.Data.GetData(DataFormats.FileDrop, false);

        // Check if filePaths is null
        if (filePaths == null) {
            ErrorHandler.ModInstallFailed(controller, "There is something wrong with the file provided, please try a different file.", false);
            return;
        }
        controller.SetProgress(0.05);

        if (filePaths.Length > 1) {
            ErrorHandler.ModInstallFailed(controller, "Please only drop one mod at a time. We currently do not support installing multiple mods at a time.", false);
            return;
        }
        controller.SetProgress(0.10);

        // Setup workDir for Operations
        controller.SetMessage("Setting Up Working Directory.");
        var workDir = System.AppDomain.CurrentDomain.BaseDirectory + "\\workdir";
        var workDirExists = Directory.Exists(workDir);
        if (!workDirExists) Directory.CreateDirectory(workDir);
        controller.SetProgress(0.15);

        // Check if The file dropped is a zip file.
        controller.SetMessage("Checking if Valid mod.zip...");
        if (Path.GetExtension(filePaths[0]) != ".zip") {
            ErrorHandler.ModInstallFailed(controller, "The file provided was not a valid mod, please try a different file.", false);
            return;
        }
        controller.SetProgress(0.20);

        // Check if the New File already exists in workdir.
        var fileName = Path.GetFileName(filePaths[0]);
        if (File.Exists(workDir + "\\" + fileName)) {
            ErrorHandler.ModInstallFailed(controller, "The working directory aready contains the file your trying to install. Please cleanup the workDir in the options menu!", false);
            return;
        }

        // Copy the File to WorkDir
        controller.SetMessage("Copying mod.zip to working directory..");
        File.Copy(filePaths[0], workDir + "\\" + fileName);
        controller.SetProgress(0.24);

        //// BELOW THIS POINT IF ERROR CLEANUP WORKDIR \\\\

        // Parse ModID from FileName
        var match = Regex.Match(fileName, @"(-\d\d-)", RegexOptions.IgnoreCase);
        string modId;
        if (match.Success) {
            modId = match.Groups[1].Value;
            modId = modId.Trim(new[] { '-' });
        }
        else {
            ErrorHandler.ModInstallFailed(controller, fileName, true);
            return;
        }
        controller.SetProgress(0.29);

        // Check if mod contains a correct structure. Create Directory to Extract Files.
        controller.SetMessage("Copying mod.zip contents to working directory.");
        var modDir = Path.GetFileNameWithoutExtension(filePaths[0]);
        var outputDir = workDir + "\\" + modDir;
        Directory.CreateDirectory(outputDir);

        // Unzip mod.zip to ModDir
        var zipPath = workDir + "\\" + fileName;
        try {
            ZipFile.ExtractToDirectory(zipPath, outputDir);
        } catch {
            ErrorHandler.ModInstallFailed(controller, "Could not extract the mod to working directory, performing cleanup of Working Directory.", true);
            return;
        }

        // Get Copy of ModInfo of the mod we are installing.
        var modToInstall = await ApiHandler.GetModById(modId);
        controller.SetProgress(0.34);

        // Get All Files we need to install
        var pluginsDlls = Directory.GetFiles(outputDir, "*.dll");
        var assetBundles = Directory.GetFiles(outputDir, "*.unity3d");
        controller.SetProgress(0.40);

        // Copy Mod in modDir to GamePath/BepInEx/
        foreach (var pluginPath in pluginsDlls) {
            var name = Path.GetFileName(pluginPath);
            modToInstall.ModFiles?.Add(name);
            // Make exception for patcher files. Currently no released mods use a patcher so can't test this with a real install.
            File.Copy(pluginPath, mv.ClientSettings.GamePath + "\\BepInEx\\plugins\\" + name);
        }

        // Copy AssetBundles to BepInEx/plugins in their correct folders.

        modToInstall.IsInstalled = true;
        mv.InstalledMods.Add(modToInstall);
        mv.ModList.Add(modToInstall);
        controller.SetProgress(0.40);

        // Save Installed Mods To File.
        DataHandler.SaveInstalledMods(mv.InstalledMods);

        // Refresh Mod List
        mv.ModList.Clear();
        ApiHandler.GetAllMods(mv);

        // Close Dialog Controller
        controller.SetMessage("Successfully installed " + modToInstall.Name + "!");
        controller.SetProgress(0.99999);
        GeneralEvents.CleanupWorkDir();
        await Task.Delay(5000);
        await controller.CloseAsync();
    }
}
