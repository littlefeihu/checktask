using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Navigation;
using Microsoft.Practices.Prism.Mvvm;
using LexisNexis.Red.Common.HelpClass;

namespace LexisNexis.Red.WindowsStore.ViewModels
{
    public class PublicationInfoPageViewModel : BaseViewModel
    {

        [RestorableState]
        public PublicationViewModel Publication { get; set; }
        [RestorableState]
        public string CustomerSupportEmail { get; set; }
        [RestorableState]
        public string CustomerSupportTEL { get; set; }

        public override void OnNavigatedTo(object navigationParameter, NavigationMode navigationMode, Dictionary<string, object> viewModelState)
        {
            base.OnNavigatedTo(navigationParameter, navigationMode, viewModelState);
            if (navigationMode == NavigationMode.New && navigationParameter != null)
            {
                var bookId = (int)navigationParameter;
                if (SessionService.SessionState.ContainsKey(ALL_PUBLICATIONS_SESSION_KEY))
                {
                    var publications =
                          SessionService.SessionState[ALL_PUBLICATIONS_SESSION_KEY] as List<PublicationViewModel>;

                    Publication = publications.FirstOrDefault(x => x.BookId == bookId);
                }
                //Publication.Expired = true;
                CustomerSupportEmail = GlobalAccess.Instance.CurrentUserInfo.Country.CustomerSupportEmail;
                CustomerSupportTEL = GlobalAccess.Instance.CurrentUserInfo.Country.CustomerSupportTEL;
            }

        }

        public void NategateTo(string page,object param)
        {
            NavigationService.Navigate(page, param);
        }

        public void GoBack()
        {
            NavigationService.GoBack();
        }
    }
}
