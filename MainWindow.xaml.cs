using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
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
using MBModManager.Models;

namespace MBModManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow {

        public Settings ClientSettings;
        public ObservableCollection<ModInfo> modList;
        private ObservableCollection<ModInfo> depsList;
        private ObservableCollection<Tag> tagsList;
        private IsEnabledState _modEnabledState;
        public List<ModInfo> InstalledMods; 

        public MainWindow() {
            
            // Load Application Settings
            ClientSettings = DataHandler.LoadAppSettings();
            InstalledMods = DataHandler.LoadInstalledMods();
            modList = new ObservableCollection<ModInfo>();
            depsList = new ObservableCollection<ModInfo>();
            tagsList = new ObservableCollection<Tag>();
            APIHandler.GetAllMods(this);
            _modEnabledState = new IsEnabledState(false);

            InitializeComponent();

            ModListing.ItemsSource = modList;
            ModListing.DataContext = this;
            ModDependenciesList.ItemsSource = depsList;
            ModDependenciesList.DataContext = this;
            ModTagsList.ItemsSource = tagsList;
            ModTagsList.DataContext = this;
            ModEnabledStatus.DataContext = _modEnabledState;

            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ModListing.ItemsSource);
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
            if (string.IsNullOrEmpty(SearchTextBox.Text)) {
                return true;
            } else {
                if (SearchTextBox.Text.StartsWith("@")) {
                    string searchBy = SearchTextBox.Text.Trim(new char[] { '@' });
                    return ((item as ModInfo).Author.IndexOf(searchBy, StringComparison.OrdinalIgnoreCase) >= 0);
                } else {
                    return ((item as ModInfo).Name.IndexOf(SearchTextBox.Text, StringComparison.OrdinalIgnoreCase) >= 0);
                }
            }
        }


        //
        // Options Section Events
        //

        private void OptionsSectionLoaded(object sender, RoutedEventArgs e) {
            SavePathBox.Text = ClientSettings.SavesPath;
            GamePathBox.Text = ClientSettings.GamePath;
        }

        private void SetGamePath(object sender, RoutedEventArgs e) {
            ClientSettings.GamePath = GamePathBox.Text;
            DataHandler.SaveAppSettings(ClientSettings);
        }

        private void SetSavePath(object sender, RoutedEventArgs e) {
            ClientSettings.SavesPath = SavePathBox.Text;
            DataHandler.SaveAppSettings(ClientSettings);
        }

        private void CleanWorkDirectory(object sender, RoutedEventArgs e) {
            GeneralEvents.CleanupWorkDir();
        }


        //
        // MainWindow Events 
        //

        private void OpenMBModding(object sender, RoutedEventArgs e) {
            GeneralEvents.OpenWeb("https://mods-monbazou.amenofisch.dev");
        }

        private void CheckUpdates(object sender, RoutedEventArgs e) { 
            GeneralEvents.OpenWeb("https://github.com/NightPotato/MBModManager");
        }

        private void LaunchGame(object sender, RoutedEventArgs e) {
            if (GeneralEvents.CheckDependencies() == false) { return; }
            Process.Start(ClientSettings.GamePath + "\\Mon Bazou.exe");
        }


        //
        // Mods Section Events
        //

        private void search_box_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) {
            if (ModListing.ItemsSource != null) {
                CollectionViewSource.GetDefaultView(ModListing.ItemsSource).Refresh();
            }
        }

        private void BepInExInstall(object sender, RoutedEventArgs e) {
            InstallEvents.InstallBepInEx(this);
        }

        private void ModListRefresh(object sender, RoutedEventArgs e) {
            APIHandler.RefreshModList(this);
        }

        //  Parse ModInfo of Selected its in Mods List for Mod Information Section.
        private void ModListingSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) {
            ModInfo selectedMod = ModListing.SelectedItem as ModInfo;
            if (selectedMod == null) { return; }

            ModNameLabel.Text = selectedMod.Name;
            ModAuthorLabel.Text = selectedMod.Author;
            ModDescriptionLabel.Text = selectedMod.Description;
            _modEnabledState.Set(selectedMod.IsEnabled);

            if (selectedMod.Tags != null) {
                tagsList.Clear();
                foreach (var tag in selectedMod.Tags) {
                    tagsList.Add(tag);
                }
            }

            if (selectedMod.DependsOn != null) {
                depsList.Clear();
                foreach (var dep in selectedMod.DependsOn) {
                    depsList.Add(dep);
                }
            }
        }

        // Drag-n-Drop Enter Check
        private void ModInstallBoxDragEnter(object sender, DragEventArgs e) {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
                e.Effects = DragDropEffects.Copy;
            }
            else {
                e.Effects = DragDropEffects.None;
            }
        }

        private void ModInstallBoxDrop(object sender, DragEventArgs e) {
            InstallEvents.InstallMod(this, e);
        }


    }



}