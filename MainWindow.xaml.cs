using MahApps.Metro.Controls;
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

namespace MBModManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow {

        public Settings clientSettings;
        private ObservableCollection<ModInfo> modList;
        private ObservableCollection<ModInfo> depsList;
        private ObservableCollection<Tag> tagsList;
        private IsEnabledState _modEnabledState;
        public List<ModInfo> InstalledMods; 

        public MainWindow() {
            
            // Load Application Settings
            clientSettings = DataHandler.LoadAppSettings();
            InstalledMods = DataHandler.LoadInstalledMods();
            modList = new ObservableCollection<ModInfo>();
            depsList = new ObservableCollection<ModInfo>();
            tagsList = new ObservableCollection<Tag>();
            APIHandler.GetAllMods(this);
            _modEnabledState = new IsEnabledState(false);

            InitializeComponent();

            installedMods.ItemsSource = modList;
            installedMods.DataContext = this;

            modInfo_Deps.ItemsSource = depsList;
            modInfo_Deps.DataContext = this;

            ModInfo_Tags.ItemsSource = tagsList;
            ModInfo_Tags.DataContext = this;

            ModInfo_enabledStatus.DataContext = _modEnabledState;

            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(installedMods.ItemsSource);
            view.Filter = SearchFilter;

        }


        public ObservableCollection<ModInfo> DepsList {
            get { return depsList; }
        }

        public ObservableCollection<ModInfo> ModList {
            get { return modList; }
        }

        public ObservableCollection<Tag> TagsList {
            get { return tagsList; }
        }

        private bool SearchFilter(object item) {
            if (string.IsNullOrEmpty(search_box.Text)) {
                return true;
            } else {
                if (search_box.Text.StartsWith("@")) {
                    string searchBy = search_box.Text.Trim(new char[] { '@' });
                    return ((item as ModInfo).Author.IndexOf(searchBy, StringComparison.OrdinalIgnoreCase) >= 0);
                } else {
                    return ((item as ModInfo).Name.IndexOf(search_box.Text, StringComparison.OrdinalIgnoreCase) >= 0);
                }
            }
        }


        //
        // Options Section Events
        //

        private void Options_Section_Loaded(object sender, RoutedEventArgs e) {
            SavePathBox.Text = clientSettings.SavesPath;
            GamePathBox.Text = clientSettings.GamePath;
        }

        private void SetGamePath(object sender, RoutedEventArgs e) {
            clientSettings.GamePath = GamePathBox.Text;
            DataHandler.SaveAppSettings(clientSettings);
        }

        private void SetSavePath(object sender, RoutedEventArgs e) {
            clientSettings.SavesPath = SavePathBox.Text;
            DataHandler.SaveAppSettings(clientSettings);
        }

        private void clear_workDir_btn_Click(object sender, RoutedEventArgs e) {
            GeneralEvents.CLEANUP_WORKDIR();
        }


        //
        // MainWindow Events 
        //

        private void openMBModding(object sender, RoutedEventArgs e) { GeneralEvents.OPEN_WEB("https://mods-monbazou.amenofisch.dev"); }

        private void checkUpdates(object sender, RoutedEventArgs e) { GeneralEvents.OPEN_WEB("https://github.com/NightPotato/MBModManager"); }

        private void LaunchGame_btn_Click(object sender, RoutedEventArgs e) {
            if (GeneralEvents.CHECK_DEPS() == false) { return; }
            Process.Start(clientSettings.GamePath + "\\Mon Bazou.exe");
        }


        //
        // Mods Section Events
        //

        private void search_box_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) {
            if (installedMods.ItemsSource != null) {
                CollectionViewSource.GetDefaultView(installedMods.ItemsSource).Refresh();
            }
        }

        private void bepinex_Install_btn_Click(object sender, RoutedEventArgs e) {
            InstallEvents.BIX_INSTALL(this);
        }

        private void refresh_API_Btn_Click(object sender, RoutedEventArgs e) {
            APIHandler.RefreshModList(this);
        }

        //  Parse ModInfo of Selected its in Mods List for Mod Information Section.
        private void installedMods_SelectionChanged_1(object sender, System.Windows.Controls.SelectionChangedEventArgs e) {
            ModInfo selectedMod = installedMods.SelectedItem as ModInfo;
            if (selectedMod == null) { return; }

            ModInfo_Name.Text = selectedMod.Name;
            ModInfo_Author.Text = selectedMod.Author;
            modInfo_Desc.Text = selectedMod.Description;

            _modEnabledState.Set(selectedMod.isEnabled);

            if (selectedMod.Tags != null) {
                tagsList.Clear();
                foreach (var tag in selectedMod.Tags) {
                    tagsList.Add(tag);
                }
            }

            if (selectedMod.depends_on != null) {
                depsList.Clear();
                foreach (var dep in selectedMod.depends_on) {
                    depsList.Add(dep);
                }
            }
        }

        // Drag-n-Drop Enter Check
        private void mod_install_box_DragEnter(object sender, DragEventArgs e) {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
                e.Effects = DragDropEffects.Copy;
            }
            else {
                e.Effects = DragDropEffects.None;
            }
        }

        private async void mod_install_box_Drop(object sender, DragEventArgs e) {
            //InstallEvents.MOD_INSTALL(this);


            // Setup Dialog Controller
            MetroDialogSettings dialogSettings = new MetroDialogSettings();
            var controller = await this.ShowProgressAsync("Installing Mod!", "Moving Mod to working-directory.", false, dialogSettings);
            controller.SetProgressBarForegroundBrush(new SolidColorBrush(Color.FromRgb(71, 125, 17)));

            // Check if GamePath is set
            if (clientSettings.GamePath == null || clientSettings.GamePath == "Path Not Set") {
                ErrorHandler.MOD_INSTALL_FAILED(controller, "GamePath was not set in options, please set the GamePath and try again.", false);
                return;
            }

            // Check if Multiple Files were dropped in the install.
            string[] filePaths = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            if (filePaths.Length > 1) {
                ErrorHandler.MOD_INSTALL_FAILED(controller, "Please only drop one mod at a time. We currently do not support installing multiple mods at a time.", false);
                return;
            }
            controller.SetProgress(0.05);
            await Task.Delay(500);

            // Check if filePaths is null
            if (filePaths == null) {
                ErrorHandler.MOD_INSTALL_FAILED(controller, "There is something wrong with the file provided, please try a different file.", false);
                return;
            }
            controller.SetProgress(0.10);
            await Task.Delay(500);

            // Setup workDir for Operations
            controller.SetMessage("Setting Up Working Directory.");
            string workDir = System.AppDomain.CurrentDomain.BaseDirectory + "\\workdir";
            bool workDirExists = Directory.Exists(workDir);
            if (!workDirExists) { Directory.CreateDirectory(workDir); }

            if (workDir == null) {
                ErrorHandler.MOD_INSTALL_FAILED(controller, "Failed to create Working Directory.", false);
                return;
            }
            controller.SetProgress(0.15);
            await Task.Delay(500);

            // Check if The file dropped is a zip file.
            controller.SetMessage("Checking if Valid mod.zip...");
            if (Path.GetExtension(filePaths[0]) != ".zip") {
                ErrorHandler.MOD_INSTALL_FAILED(controller, "The file provided was not a valid mod, please try a different file.", false);
                return;
            }
            controller.SetProgress(0.20);
            await Task.Delay(500);

            // Check if the New File already exists in workdir.
            string fileName = Path.GetFileName(filePaths[0]);
            if (File.Exists(workDir + "\\" + fileName)) {
                ErrorHandler.MOD_INSTALL_FAILED(controller, "The working directory aready contains the file your trying to install. Please cleanup the workDir in the options menu!", false);
                return;
            }

            // Copy the File to WorkDir
            controller.SetMessage("Copying mod.zip to working directory..");
            File.Copy(filePaths[0], workDir + "\\" + fileName);
            controller.SetProgress(0.24);
            await Task.Delay(500);

            //// BELOW THIS POINT IF ERROR CLEANUP WORKDIR \\\\

            // Parse ModID from FileName
            Match match = Regex.Match(fileName, @"(-\d\d-)", RegexOptions.IgnoreCase);
            string modId;
            if (match.Success) {
                modId = match.Groups[1].Value;
                modId = modId.Trim(new char[] { '-' });
            } else {
                ErrorHandler.MOD_INSTALL_FAILED(controller, fileName, true);
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
                ErrorHandler.MOD_INSTALL_FAILED(controller, "Could not extract the mod to working directory, performing cleanup of Working Directory.", true);
                return;
            }

            // Get Copy of ModInfo of the mod we are installing.
            ModInfo ModToInstall = await APIHandler.GetModByID(this, modId);
            controller.SetProgress(0.34);
            await Task.Delay(500);

            // Get All Files we need to install
            string[] pluginsDlls = Directory.GetFiles(outputDir, "*.dll");
            string[] assetBundles = Directory.GetFiles(outputDir, "*.unity3d");
            controller.SetProgress(0.40);
            await Task.Delay(500);

            // Copy Mod in modDir to GamePath/BepInEx/
            foreach (string pluginPath in pluginsDlls) {
                string _fileName = Path.GetFileName(pluginPath);
                ModToInstall.ModFiles.Add(_fileName);
                // Make exception for patcher files. Currently no released mods use a patcher so can't test this with a real install.
                File.Copy(pluginPath, clientSettings.GamePath + "\\BepInEx\\plugins\\" + _fileName);
            }

            // Copy AssetBundles to BepInEx/plugins in their correct folders.

            ModToInstall.isInstalled = true;
            InstalledMods.Add(ModToInstall);
            modList.Add(ModToInstall);
            controller.SetProgress(0.40);
            await Task.Delay(500);

            // Save Installed Mods To File.
            DataHandler.SaveInstalledMods(InstalledMods);

            // Refresh Mod List
            modList.Clear();
            APIHandler.GetAllMods(this);

            // Close Dialog Controller
            controller.SetMessage("Successfully installed " + ModToInstall.Name + "!");
            controller.SetProgress(0.99999);
            await Task.Delay(5000);
            await controller.CloseAsync();

        }

    }



}