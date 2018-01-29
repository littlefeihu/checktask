using System;
using System.Threading.Tasks;
using Windows.UI.Popups;
using LexisNexis.Red.WindowsStore.Controls;

namespace LexisNexis.Red.WindowsStore.Services
{
    public class AlertMessageService : IAlertMessageService
    {
        private static bool IsShowing;

        public async Task ShowAsync(string message, string title)
        {
            await ShowAsync(message, title, null);
        }

        public async Task ShowAsync(string message, string title, params UICommand[] dialogCommands)
        {
            // Only show one dialog at a time.
            if (!IsShowing)
            {
                var messageDialog = new CustomMessageDialog(message, title);

                if (dialogCommands != null)
                {
                    var commands = dialogCommands;

                    foreach (var command in commands)
                    {
                        messageDialog.Commands.Add(command);
                    }
                }

                IsShowing = true;
                await messageDialog.ShowAsync();
                IsShowing = false;
            }
        }
    }
}
