
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace LexisNexis.Red.WindowsStore.Services
{
    public interface IAlertMessageService
    {
        Task ShowAsync(string message, string title);

        Task ShowAsync(string message, string title, params UICommand[] dialogCommands);
    }
}
