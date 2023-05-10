using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

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

        }


        //
        // Mods Section Events
        //

        private async void bepinex_Install_btn_Click(object sender, System.Windows.RoutedEventArgs e) {

            var controller = await this.ShowProgressAsync("Please wait...", "Downloading BepInEx from Github Releases.");

            await Task.Delay(2000);
            using (var client = new HttpClient()) {
                controller.SetProgress(0.25f);
                var response = await client.GetByteArrayAsync("https://github.com/BepInEx/BepInEx/releases/download/v5.4.21/BepInEx_x64_5.4.21.0.zip");
                File.WriteAllBytes(System.AppDomain.CurrentDomain.BaseDirectory + "BepInEx.zip", response);
                controller.SetMessage("Finished Downloading BepInEx.");
            }

            await Task.Delay(3000);
            controller.SetMessage("Installing BepInEx to Game Directory.");
            controller.SetProgress(0.45f);

            await Task.Delay(15000);
            await controller.CloseAsync();

            // GetLatestRelease from Github
            // Unzip Release in GamePath
            // Display Status of Install

        }


    }



}