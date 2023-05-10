using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MBModManager {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow {

        public Settings clientSettings;

        public  MainWindow() {
            
            // Load Application Settings
            clientSettings = DataManager.LoadAppSettings();
            InitializeComponent();
        }


        //
        // Options Section Events
        //

        private void Options_Section_Loaded(object sender, System.Windows.RoutedEventArgs e) {
            SavePathBox.Text = clientSettings.SavesPath;
            GamePathBox.Text = clientSettings.GamePath;
        }

        private void SetGamePath(object sender, System.Windows.RoutedEventArgs e) {
            clientSettings.GamePath = GamePathBox.Text;
            DataManager.SaveAppSettings(clientSettings);
        }

        private void SetSavePath(object sender, System.Windows.RoutedEventArgs e) {
            clientSettings.SavesPath = SavePathBox.Text;
            DataManager.SaveAppSettings(clientSettings);
        }


        //
        // MainWindow Events 
        //

        private void openMBModding(object sender, System.Windows.RoutedEventArgs e) {
            Process.Start(new ProcessStartInfo("https://mods-monbazou.amenofisch.dev") { UseShellExecute = true });
        }

        
        private void checkUpdates(object sender, System.Windows.RoutedEventArgs e) {
            // System.Diagnostics.Process.Start("https://github.com/NightPotato/MBModManager");
        }

        private void LaunchGame_btn_Click(object sender, System.Windows.RoutedEventArgs e) {
            // Check if all Mods have the required dependencies installed and loaded.
            // Check if UnityExplorer is installed and loaded else install and load it.
            Process.Start(clientSettings.GamePath + "\\Mon Bazou.exe");
        }


        //
        // Mods Section Events
        //

        private async void bepinex_Install_btn_Click(object sender, System.Windows.RoutedEventArgs e) {

            MetroDialogSettings dialogSettings = new MetroDialogSettings();
            dialogSettings.ColorScheme = MetroDialogColorScheme.Inverted;
            var controller = await this.ShowProgressAsync("Please wait...", "Downloading BepInEx from Github Releases.", false, dialogSettings);
            controller.SetProgressBarForegroundBrush(new System.Windows.Media.SolidColorBrush(Color.FromRgb(71, 125, 17)));

            string zipPath = System.AppDomain.CurrentDomain.BaseDirectory + "BepInEx.zip";

            if (clientSettings.InternalData == null || clientSettings.InternalData[2] == null) {
                ErrorHandlers.BIX_INSTALL_FAILED(controller, "Some of the applications internal data is missing, please delete settings.json and relaunch MBModManager.");
                return;
            }

            // Check if the GamePath is set in the Options.
            if (clientSettings.GamePath == null || clientSettings.GamePath == "") {
                ErrorHandlers.BIX_INSTALL_FAILED(controller, "The Game Path was not set in the options. Please set a Game Path before installing a BepInEx.");
                return;
            }

            // GetLatestRelease from Github
            await Task.Delay(2000);
            using (var client = new HttpClient()) {
                controller.SetProgress(0.25f);
                var response = await client.GetByteArrayAsync(clientSettings.InternalData[2]);
                File.WriteAllBytes(zipPath, response);
                controller.SetProgress(0.35f);
                controller.SetMessage("Finished Downloading BepInEx.");
            }

            // Unzip Release in GamePath
            await Task.Delay(2000);
            controller.SetMessage("Installing BepInEx to Game Directory.");
            controller.SetProgress(0.45f);

            try {
                ZipFile.ExtractToDirectory(zipPath, clientSettings.GamePath);
            } catch {
                ErrorHandlers.BIX_INSTALL_FAILED(controller, "Game already contains some of BepInEx files. Please remove these files from your game before installing.");
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