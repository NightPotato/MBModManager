using MahApps.Metro.Controls.Dialogs;
using MBModManager.Events;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MBModManager.Handlers;

internal static class ErrorHandler
{
    public static async void BepInExInstallFailed(ProgressDialogController controller, string message)
    {
        controller.SetProgressBarForegroundBrush(new SolidColorBrush(Color.FromRgb(183, 35, 35)));
        controller.SetProgress(1f);
        controller.SetTitle("Oops.. We ran into an Error!");
        controller.SetMessage(message);
        await Task.Delay(5000);
        await controller.CloseAsync();
    }

    public static async void ModInstallFailed(ProgressDialogController controller, string message, bool shouldCleanUp)
    {
        controller.SetProgressBarForegroundBrush(new SolidColorBrush(Color.FromRgb(183, 35, 35)));
        controller.SetProgress(1f);
        controller.SetTitle("Opps. Mod Install Failed!");
        controller.SetMessage(message);
        if (shouldCleanUp) { GeneralEvents.CleanupWorkDir(); }
        await Task.Delay(5000);
        await controller.CloseAsync();
    }
}