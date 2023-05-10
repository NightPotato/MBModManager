using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MBModManager {
    internal class ErrorHandlers {

        public async static void BIX_INSTALL_FAILED(ProgressDialogController controller, string message) {
            controller.SetProgressBarForegroundBrush(new System.Windows.Media.SolidColorBrush(Color.FromRgb(183, 35, 35)));
            controller.SetProgress(1f);
            controller.SetTitle("Opps.. We ran into an Error!");
            controller.SetMessage(message);
            await Task.Delay(5000);
            await controller.CloseAsync();
        }

    }
}
