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

        public DropInstallControl() {
            InitializeComponent();
        }

        private void DropInstallControl_DragEnter(object sender, DragEventArgs e) {
            e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
        }

        private void DropInstallControl_Drop(object sender, DragEventArgs e) {
            var window = (Application.Current.MainWindow as MainWindow);
            if (window == null) return;
            InstallEvents.InstallMod(window, e);
        }
    }
}
