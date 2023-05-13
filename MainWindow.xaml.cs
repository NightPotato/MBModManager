using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MBModManager.Data;
using MBModManager.Events;
using MBModManager.Handlers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MBModManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow {

        public Settings clientSettings;
        private ObservableCollection<ModInfo> modList;

        public  MainWindow() {
            
            // Load Application Settings
            clientSettings = DataHandler.LoadAppSettings();
            modList = DataHandler.GetModList();

            InitializeComponent();
            installedMods.ItemsSource = modList;
            installedMods.DataContext = this;

        }


        public ObservableCollection<ModInfo> ModList {
            get {
                return modList;
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

        private void bepinex_Install_btn_Click(object sender, RoutedEventArgs e) {
            InstallEvents.BIX_INSTALL(this);
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
            dialogSettings.ColorScheme = MetroDialogColorScheme.Inverted;
            var controller = await this.ShowProgressAsync("Installing Mod!", "Moving mod.zip to working-directory.", false, dialogSettings);
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
            string workDir = System.AppDomain.CurrentDomain.BaseDirectory + "\\workdir";
            bool workDirExists = Directory.Exists(workDir);

            if (!workDirExists) {
                Directory.CreateDirectory(workDir);
            }

            if (workDir == null) {
                ErrorHandler.MOD_INSTALL_FAILED(controller, "Failed to create Working Directory.", false);
                return;
            }

            controller.SetProgress(0.15);
            await Task.Delay(500);

            // Check if The file dropped is a zip file.
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
            File.Copy(filePaths[0], workDir + "\\" + fileName);
            controller.SetProgress(0.24);
            await Task.Delay(500);

            // BELOW THIS POINT IF ERROR CLEANUP WORKDIR


            // Check if mod contains a correct structure.
            // Create Directory to Extract Files
            string modDir = Path.GetFileNameWithoutExtension(filePaths[0]);
            string outputDir = workDir + "\\" + modDir;
            Directory.CreateDirectory(outputDir);

            // Unzip mod.zip to ModDir
            string zipPath = workDir + "\\" + fileName;
            try {
                ZipFile.ExtractToDirectory(zipPath, outputDir);
            } catch {
                Handlers.ErrorHandler.MOD_INSTALL_FAILED(controller, "Could not extract the mod to working directory, performing cleanup of Working Directory.", true);
                return;
            }

            // Gather ModInfo for the mod being installed and check if it is currently a hosted mod within the API.
            // BackpackMod-19-1-0-3-1659649243.zip


            // Copy Mod in modDir to GamePath/BepInEx/


            // Close Dialog Controller
            controller.SetProgress(0.99999);
            await Task.Delay(5000);
            await controller.CloseAsync();

        }

    }



}