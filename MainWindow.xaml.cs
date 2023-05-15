using MahApps.Metro.Controls.Dialogs;
using MBModManager.Data;
using MBModManager.Events;
using MBModManager.Handlers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace MBModManager;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{

    public readonly List<ModInfo> InstalledMods;
    public readonly Settings ClientSettings;
    public ObservableCollection<ModInfo> ModList { get; }
    private ObservableCollection<Tag> TagsList { get; }
    private ObservableCollection<ModInfo> DepsList { get; }
    private readonly IsEnabledState _modEnabledState;

    public MainWindow()
    {
        // Load Application Settings
        ClientSettings = DataHandler.LoadAppSettings();
        InstalledMods = DataHandler.LoadInstalledMods();
        ModList = new ObservableCollection<ModInfo>();
        DepsList = new ObservableCollection<ModInfo>();
        TagsList = new ObservableCollection<Tag>();
        ApiHandler.GetAllMods(this);
        _modEnabledState = new IsEnabledState(false);

        InitializeComponent();

        installedMods.ItemsSource = ModList;
        installedMods.DataContext = this;

        modInfo_Deps.ItemsSource = DepsList;
        modInfo_Deps.DataContext = this;

        ModInfo_Tags.ItemsSource = TagsList;
        ModInfo_Tags.DataContext = this;

        ModInfo_enabledStatus.DataContext = _modEnabledState;

        var view = (CollectionView)CollectionViewSource.GetDefaultView(installedMods.ItemsSource);
        view.Filter = SearchFilter;
    }

    private bool SearchFilter(object item)
    {
        if (string.IsNullOrEmpty(search_box.Text) || item is not ModInfo modInfo)
        {
            return true;
        }

        if (!search_box.Text.StartsWith("@") || string.IsNullOrWhiteSpace(modInfo.Author))
        {
            return string.IsNullOrWhiteSpace(modInfo.Name) ||
                   modInfo.Name.Contains(search_box.Text, StringComparison.OrdinalIgnoreCase);
        }

        var searchBy = search_box.Text.TrimStart('@');
        return modInfo.Author.Contains(searchBy, StringComparison.OrdinalIgnoreCase);
    }

    //
    // Options Section Events
    //

    private void OptionsSectionLoaded(object sender, RoutedEventArgs e)
    {
        SavePathBox.Text = ClientSettings.SavesPath;
        GamePathBox.Text = ClientSettings.GamePath;
    }

    private void SetGamePath(object sender, RoutedEventArgs e)
    {
        ClientSettings.GamePath = GamePathBox.Text;
        DataHandler.SaveAppSettings(ClientSettings);
    }

    private void SetSavePath(object sender, RoutedEventArgs e)
    {
        ClientSettings.SavesPath = SavePathBox.Text;
        DataHandler.SaveAppSettings(ClientSettings);
    }

    private void ClearWorkDirBtnClick(object sender, RoutedEventArgs e)
    {
        GeneralEvents.CleanupWorkDir();
    }

    //
    // MainWindow Events
    //

    private void OpenMbModding(object sender, RoutedEventArgs e) => GeneralEvents.OpenWeb("https://mods-monbazou.amenofisch.dev");

    private void CheckUpdates(object sender, RoutedEventArgs e) => GeneralEvents.OpenWeb("https://github.com/NightPotato/MBModManager");

    private void LaunchGameBtnClick(object sender, RoutedEventArgs e)
    {
        if (GeneralEvents.CheckDependencies() == false) { return; }
        Process.Start(ClientSettings.GamePath + "\\Mon Bazou.exe");
    }

    //
    // Mods Section Events
    //

    private void SearchBoxTextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
        if (installedMods.ItemsSource != null) {
            CollectionViewSource.GetDefaultView(installedMods.ItemsSource).Refresh();
        }
    }

    private void BepInExInstallBtnClick(object sender, RoutedEventArgs e)
    {
        InstallEvents.InstallBepInEx(this);
    }

    private void RefreshApiBtnClick(object sender, RoutedEventArgs e)
    {
        ApiHandler.RefreshModList(this);
    }

    //  Parse ModInfo of Selected its in Mods List for Mod Information Section.
    private void InstalledModsSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        if (installedMods.SelectedItem is not ModInfo selectedMod) { return; }

        ModInfo_Name.Text = selectedMod.Name;
        ModInfo_Author.Text = selectedMod.Author;
        modInfo_Desc.Text = selectedMod.Description;

        _modEnabledState.Set(selectedMod.isEnabled);

        if (selectedMod.Tags != null) {
            TagsList.Clear();
            foreach (var tag in selectedMod.Tags) {
                TagsList.Add(tag);
            }
        }

        if (selectedMod.depends_on == null) return;
        DepsList.Clear();
        foreach (var dep in selectedMod.depends_on) {
            DepsList.Add(dep);
        }
    }

    // Drag-n-Drop Enter Check
    private void ModInstallBoxDragEnter(object sender, DragEventArgs e)
    {
        e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
    }

    private async void ModInstallBoxDrop(object sender, DragEventArgs e)
    {
        //InstallEvents.MOD_INSTALL(this);


        // Setup Dialog Controller
        var dialogSettings = new MetroDialogSettings();
        var controller = await this.ShowProgressAsync("Installing Mod!", "Moving Mod to working-directory.", false, dialogSettings);
        controller.SetProgressBarForegroundBrush(new SolidColorBrush(Color.FromRgb(71, 125, 17)));

        // Check if GamePath is set
        if (ClientSettings.GamePath is null or "Path Not Set") {
            ErrorHandler.ModInstallFailed(controller, "GamePath was not set in options, please set the GamePath and try again.", false);
            return;
        }

        // Check if Multiple Files were dropped in the install.
        if (e.Data.GetData(DataFormats.FileDrop, false) is not string[] filePaths) {
            ErrorHandler.ModInstallFailed(controller, "There is something wrong with the file provided, please try a different file.", false);
            return;
        }

        controller.SetProgress(0.05);
        await Task.Delay(500);

        // Check if filePaths is null
        if (filePaths.Length > 1) {
            ErrorHandler.ModInstallFailed(controller, "Please only drop one mod at a time. We currently do not support installing multiple mods at a time.", false);
            return;
        }
        controller.SetProgress(0.10);
        await Task.Delay(500);

        // Setup workDir for Operations
        controller.SetMessage("Setting Up Working Directory.");
        var workDir = AppDomain.CurrentDomain.BaseDirectory + "\\workdir";
        var workDirExists = Directory.Exists(workDir);
        if (!workDirExists) Directory.CreateDirectory(workDir);

        controller.SetProgress(0.15);
        await Task.Delay(500);

        // Check if The file dropped is a zip file.
        controller.SetMessage("Checking if Valid mod.zip...");
        if (Path.GetExtension(filePaths[0]) != ".zip") {
            ErrorHandler.ModInstallFailed(controller, "The file provided was not a valid mod, please try a different file.", false);
            return;
        }
        controller.SetProgress(0.20);
        await Task.Delay(500);

        // Check if the New File already exists in workdir.
        var fileName = Path.GetFileName(filePaths[0]);
        if (File.Exists(workDir + "\\" + fileName)) {
            ErrorHandler.ModInstallFailed(controller, "The working directory already contains the file your trying to install. Please cleanup the workDir in the options menu!", false);
            return;
        }

        // Copy the File to WorkDir
        controller.SetMessage("Copying mod.zip to working directory..");
        File.Copy(filePaths[0], workDir + "\\" + fileName);
        controller.SetProgress(0.24);
        await Task.Delay(500);

        //// BELOW THIS POINT IF ERROR CLEANUP WORKDIR \\\\

        // Parse ModID from FileName
        var match = Regex.Match(fileName, @"(-\d\d-)", RegexOptions.IgnoreCase);
        string modId;
        if (match.Success) {
            modId = match.Groups[1].Value;
            modId = modId.Trim(new[] { '-' });
        } else {
            ErrorHandler.ModInstallFailed(controller, fileName, true);
            return;
        }
        controller.SetProgress(0.29);
        await Task.Delay(500);

        // Check if mod contains a correct structure. Create Directory to Extract Files.
        controller.SetMessage("Copying mod.zip contents to working directory.");
        string modDir = Path.GetFileNameWithoutExtension(filePaths[0]);
        string outputDir = workDir + "\\" + modDir;
        Directory.CreateDirectory(outputDir);

        // Unzip mod.zip to ModDir
        string zipPath = workDir + "\\" + fileName;
        try {
            ZipFile.ExtractToDirectory(zipPath, outputDir);
        } catch {
            ErrorHandler.ModInstallFailed(controller, "Could not extract the mod to working directory, performing cleanup of Working Directory.", true);
            return;
        }

        // Get Copy of ModInfo of the mod we are installing.
        var modToInstall = await ApiHandler.GetModById(this, modId);
        controller.SetProgress(0.34);
        await Task.Delay(500);

        // Get All Files we need to install
        var pluginsDlls = Directory.GetFiles(outputDir, "*.dll");
        var assetBundles = Directory.GetFiles(outputDir, "*.unity3d");
        controller.SetProgress(0.40);
        await Task.Delay(500);

        // Copy Mod in modDir to GamePath/BepInEx/
        foreach (var pluginPath in pluginsDlls) {
            var filesName = Path.GetFileName(pluginPath);
            modToInstall.ModFiles.Add(fileName);
            // Make exception for patcher files. Currently no released mods use a patcher so can't test this with a real install.
            File.Copy(pluginPath, ClientSettings.GamePath + "\\BepInEx\\plugins\\" + fileName);
        }

        // Copy AssetBundles to BepInEx/plugins in their correct folders.

        modToInstall.isInstalled = true;
        InstalledMods.Add(modToInstall);
        ModList.Add(modToInstall);
        controller.SetProgress(0.40);
        await Task.Delay(500);

        // Save Installed Mods To File.
        DataHandler.SaveInstalledMods(InstalledMods);

        // Refresh Mod List
        ModList.Clear();
        ApiHandler.GetAllMods(this);

        // Close Dialog Controller
        controller.SetMessage("Successfully installed " + modToInstall.Name + "!");
        controller.SetProgress(0.99999);
        GeneralEvents.CleanupWorkDir();
        await Task.Delay(5000);
        await controller.CloseAsync();

    }
}
