using MahApps.Metro.Controls.Dialogs;
using MBModManager.Events;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MBModManager.Handlers;

internal static class ErrorHandler
{
    public static async void ShowError(ProgressDialogController controller, string title = "An error occured!", string message = "An error has occured, please contact the developer!", bool shouldCleanUpWorkingDir = false)
    {
        controller.SetProgressBarForegroundBrush(new SolidColorBrush(Color.FromRgb(183, 35, 35)));
        controller.SetProgress(1f);
        controller.SetTitle(title);
        controller.SetMessage(message);
        if (shouldCleanUpWorkingDir) GeneralEvents.CleanupWorkDir();
        await Task.Delay(5000);
        await controller.CloseAsync();
    }
}
