using MBModManager.Events;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace MBModManager.Controls {
    /// <summary>
    /// Interaction logic for ModInstallBox.xaml
    /// </summary>
    public partial class DropInstallControl : UserControl {

        // TODO: Implement Property to switch between Mod or Save install.
        public enum InstallTypes { MOD, SAVE }

        [DisplayName("Installer Method"), Description("Change the Text in the Label."), Category("Installer Properties"), DefaultValue(InstallTypes.MOD)]
        public InstallTypes InstallType { get; set; }

        [DisplayName("Label Text"), Description("Change the Text in the Label."), Category("Installer Properties")]
        public string LabelText {
            get => DropInstallBoxLabel.Content.ToString();
            set => DropInstallBoxLabel.Content = value;
        }

        public DropInstallControl() {
            InitializeComponent();
        }

        private void DropInstallControl_DragEnter(object sender, DragEventArgs e) {
            e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
        }

        private void DropInstallControl_Drop(object sender, DragEventArgs e) {

            var window = (Application.Current.MainWindow as MainWindow);
            if (window == null) return;

            switch (InstallType) {
                case InstallTypes.SAVE:
                    // Add SaveFile Install
                    break;
                case InstallTypes.MOD:
                    InstallEvents.InstallMod(window, e);
                    break;
            }
        }
    }
}
