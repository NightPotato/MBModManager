using MBModManager.Events;
using MBModManager.Handlers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using MBModManager.Models;

namespace MBModManager;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    public readonly Settings ClientSettings;
    public ObservableCollection<ModInfo> ModList { get; }
    public readonly List<ModInfo> InstalledMods;
    private ObservableCollection<ModInfo> DepsList { get; }
    private ObservableCollection<Tag> TagsList { get; }
    private readonly IsEnabledState _modEnabledState;


    public MainWindow()
    {
        // Load Application Settings
        ClientSettings = DataHandler.LoadAppSettings();
        InstalledMods = DataHandler.LoadInstalledMods();
        ModList = new ObservableCollection<ModInfo>();
        DepsList = new ObservableCollection<ModInfo>();
        TagsList = new ObservableCollection<Tag>();


        // Get all mods from API and add them to the ModList
        ModInfo[] mods = ApiHandler.GetAllMods();
        foreach (ModInfo mod in mods)
        {
            if (ModList.Contains(mod)) return;
            ModList.Add(mod);
        }

        _modEnabledState = new IsEnabledState(false);

        InitializeComponent();

        ModListing.ItemsSource = ModList;
        ModListing.DataContext = this;
        ModDependenciesList.ItemsSource = DepsList;
        ModDependenciesList.DataContext = this;
        ModTagsList.ItemsSource = TagsList;
        ModTagsList.DataContext = this;
        ModEnabledStatus.DataContext = _modEnabledState;

        var view = (CollectionView)CollectionViewSource.GetDefaultView(ModListing.ItemsSource);
        view.Filter = SearchFilter;
    }

    private bool SearchFilter(object item)
    {
        if (string.IsNullOrEmpty(SearchTextBox.Text) || item is not ModInfo modInfo)
        {
            return true;
        }

        if (!SearchTextBox.Text.StartsWith("@") || string.IsNullOrWhiteSpace(modInfo.Author))
        {
            return string.IsNullOrWhiteSpace(modInfo.Name) || modInfo.Name.Contains(SearchTextBox.Text, StringComparison.OrdinalIgnoreCase);
        }

        var searchBy = SearchTextBox.Text.TrimStart('@');
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

    private void CleanWorkDirectory(object sender, RoutedEventArgs e)
    {
        GeneralEvents.CleanupWorkDir();
    }

    //
    // MainWindow Events
    //

    private void OpenMbModding(object sender, RoutedEventArgs e)
    {
        GeneralEvents.OpenWeb("https://mods-monbazou.amenofisch.dev");
    }

    private void CheckUpdates(object sender, RoutedEventArgs e)
    {
        GeneralEvents.OpenWeb("https://github.com/NightPotato/MBModManager");
    }

    private void LaunchGame(object sender, RoutedEventArgs e)
    {
        if (GeneralEvents.CheckDependencies() == false)
        {
            return;
        }
        Process.Start(ClientSettings.GamePath + "\\Mon Bazou.exe");
    }

    //
    // Mods Section Events
    //

    private void SearchBoxTextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
        if (ModListing.ItemsSource != null)
        {
            CollectionViewSource.GetDefaultView(ModListing.ItemsSource).Refresh();
        }
    }

    private void BepInExInstall(object sender, RoutedEventArgs e)
    {
        InstallEvents.InstallBepInEx(this);
    }

    private void ModListRefresh(object sender, RoutedEventArgs e)
    {
        var mods = ApiHandler.GetAllMods();

        ModList.Clear();

        foreach (var mod in mods)
        {
            if (ModList.Contains(mod)) continue;
            ModList.Add(mod);
        }
    }

    //  Parse ModInfo of Selected its in Mods List for Mod Information Section.
    private void ModListingSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        if (ModListing.SelectedItem is not ModInfo selectedMod)
        {
            return;
        }

        ModNameLabel.Text = selectedMod.Name;
        ModAuthorLabel.Text = selectedMod.Author;
        ModDescriptionLabel.Text = selectedMod.Description;
        _modEnabledState.Set(selectedMod.IsEnabled);

        if (selectedMod.Tags != null)
        {
            TagsList.Clear();
            foreach (var tag in selectedMod.Tags)
            {
                TagsList.Add(tag);
            }
        }

        if (selectedMod.DependsOn == null) return;
        DepsList.Clear();
        foreach (var dep in selectedMod.DependsOn)
        {
            DepsList.Add(dep);
        }
    }

    // Drag-n-Drop Enter Check
    private void ModInstallBoxDragEnter(object sender, DragEventArgs e)
    {
        e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
    }

    private void ModInstallBoxDrop(object sender, DragEventArgs e)
    {
        InstallEvents.InstallMod(this, e);
    }
}
