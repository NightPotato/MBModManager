using MahApps.Metro.Controls;
using MBModManager.Events;
using System.Diagnostics;

namespace MBModManager {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow {

        public Settings clientSettings;

        public  MainWindow() {
            
            // Load Application Settings
            clientSettings = Handlers.DataHandler.LoadAppSettings();
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
            Handlers.DataHandler.SaveAppSettings(clientSettings);
        }

        private void SetSavePath(object sender, System.Windows.RoutedEventArgs e) {
            clientSettings.SavesPath = SavePathBox.Text;
            Handlers.DataHandler.SaveAppSettings(clientSettings);
        }


        //
        // MainWindow Events 
        //

        private void openMBModding(object sender, System.Windows.RoutedEventArgs e) { GeneralEvents.OPEN_WEB("https://mods-monbazou.amenofisch.dev"); }
        private void checkUpdates(object sender, System.Windows.RoutedEventArgs e) { GeneralEvents.OPEN_WEB("https://github.com/NightPotato/MBModManager"); }

        private void LaunchGame_btn_Click(object sender, System.Windows.RoutedEventArgs e) {
            if (GeneralEvents.CHECK_DEPS() == false) { return; }
            Process.Start(clientSettings.GamePath + "\\Mon Bazou.exe");
        }


        //
        // Mods Section Events
        //

        private void bepinex_Install_btn_Click(object sender, System.Windows.RoutedEventArgs e) {
            InstallEvents.BIX_INSTALL(this);
        }

    }



}