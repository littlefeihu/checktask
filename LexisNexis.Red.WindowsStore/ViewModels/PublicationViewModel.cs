using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Popups;
using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.WindowsStore.Events;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.PubSubEvents;
using LexisNexis.Red.Common.HelpClass;
using Microsoft.Practices.Prism.Mvvm.Interfaces;

namespace LexisNexis.Red.WindowsStore.ViewModels
{

    public class PublicationViewModel : BaseBindableViewModel
    {

        private CancellationTokenSource tokenSource;

        public Task<DownloadResult> DownloadTask { get; private set; }


        #region Depency services


        #endregion

        #region Commands

        public DelegateCommand DownloadCommand
        {
            get;
            private set;
        }

        public DelegateCommand CancelDownloadCommand
        {
            get;
            private set;
        }

        public DelegateCommand DeleteCommand
        {
            get;
            private set;
        }

        public DelegateCommand ShowInfoCommand
        {
            get;
            private set;
        }

        public DelegateCommand GotoConentCommand
        {
            get;
            private set;
        }

        public DelegateCommand ClearDownloadErrorCommand
        {
            get;
            private set;
        }
        #endregion

        #region Properties

        private int bookId;
        public int BookId
        {
            get { return bookId; }
            set { SetProperty(ref bookId, value); }
        }

        private string title;
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        private string fullTitle;
        public string FullTitle
        {
            get { return fullTitle; }
            set { SetProperty(ref fullTitle, value); }
        }

        private string author;
        public string Author
        {
            get { return author; }
            set { SetProperty(ref author, value); }
        }

        private string primaryColor;
        public string PrimaryColor
        {
            get { return primaryColor; }
            set { SetProperty(ref primaryColor, value); }
        }

        private string secondaryColor;
        public string SecondaryColor
        {
            get { return secondaryColor; }
            set { SetProperty(ref secondaryColor, value); }
        }

        private string fontColor;
        public string FontColor
        {
            get { return fontColor; }
            set { SetProperty(ref fontColor, value); }
        }

        private bool showDownloadBtn;
        public bool ShowDownloadBtn
        {
            get { return showDownloadBtn; }
            set { SetProperty(ref showDownloadBtn, value); }
        }

        private bool showBookStatus = true;
        public bool ShowBookStatus
        {
            get { return showBookStatus; }
            set { SetProperty(ref showBookStatus, value); }
        }


        private bool downloadFailed;
        public bool DownloadFailed
        {
            get { return downloadFailed; }
            set { SetProperty(ref downloadFailed, value); }
        }

        private string downloadFailedLabel;
        public string DownloadFailedLabel
        {
            get { return downloadFailedLabel; }
            set { SetProperty(ref downloadFailedLabel, value); }
        }

        private bool isDownloading;
        public bool IsDownloading
        {
            get { return isDownloading; }
            private set { SetProperty(ref isDownloading, value); }
        }

        private string downloadingLabel;
        public string DownloadingLabel
        {
            get { return downloadingLabel; }
            private set { SetProperty(ref downloadingLabel, value); }
        }


        private int downloadProgress;
        public int DownloadProgress
        {
            get { return downloadProgress; }
            set { SetProperty(ref downloadProgress, value); }
        }

        public long Size { get; set; }

        private string sizeInfo;
        public string SizeInfo
        {
            get { return sizeInfo; }
            set { SetProperty(ref sizeInfo, value); }
        }


        private string bookStatusLabel1;
        public string BookStatusLabel1
        {
            get { return bookStatusLabel1; }
            set { SetProperty(ref bookStatusLabel1, value); }
        }

        private string bookStatusLabel2;
        public string BookStatusLabel2
        {
            get { return bookStatusLabel2; }
            set { SetProperty(ref bookStatusLabel2, value); }
        }


        private bool isLoan;
        public bool IsLoan
        {
            get { return isLoan; }
            set { SetProperty(ref isLoan, value); }
        }

        private bool showLoanInfo;
        public bool ShowLoanInfo
        {
            get { return showLoanInfo; }
            set { SetProperty(ref showLoanInfo, value); }
        }

        private string loanInfo;
        public string LoanInfo
        {
            get { return loanInfo; }
            set { SetProperty(ref loanInfo, value); }
        }

        private int daysRemaining;
        public int DaysRemaining
        {
            get { return daysRemaining; }
            set { SetProperty(ref daysRemaining, value); }
        }


        private int updateCount;
        public int UpdateCount
        {
            set { SetProperty(ref updateCount, value); }
            get { return updateCount; }
        }

        private string lastUpdateDate;
        public string LastUpdateDate
        {
            set { SetProperty(ref lastUpdateDate, value); }
            get { return lastUpdateDate; }
        }

        public PublicationStatusEnum PublicationStatus { get; set; }


        private string currentVersion;
        public string Version
        {
            get { return currentVersion; }
            set { SetProperty(ref currentVersion, value); }
        }

        private string installedDate;
        public string InstalledDate
        {
            get { return installedDate; }
            set { SetProperty(ref installedDate, value); }
        }


        private string currencyDate;
        public string CurrencyDate
        {
            get { return currencyDate; }
            set { SetProperty(ref currencyDate, value); }
        }

        private string practiceArea;
        public string PracticeArea
        {
            get { return practiceArea; }
            set { SetProperty(ref practiceArea, value); }
        }


        private string subcategory;
        public string Subcategory
        {
            get { return subcategory; }
            set { SetProperty(ref subcategory, value); }
        }


        private string description;

        public string Description
        {
            get { return description; }
            set { SetProperty(ref description, value); }
        }

        private bool canShowInfo;
        public bool CanShowInfo
        {
            get { return canShowInfo; }
            set { SetProperty(ref canShowInfo, value); }
        }

        private long localSize;
        public long LocalSize
        {
            get { return localSize; }
            set { SetProperty(ref localSize, value); }
        }

        private string localSizeInfo;
        public string LocalSizeInfo
        {
            get { return localSizeInfo; }
            set { SetProperty(ref localSizeInfo, value); }
        }


        private string casesInfo;
        public string CasesInfo
        {
            get { return casesInfo; }
            set { SetProperty(ref casesInfo, value); }
        }



        private string guideCardsAdded;
        public string GuideCardsAdded
        {
            get { return guideCardsAdded; }
            set { SetProperty(ref guideCardsAdded, value); }
        }

        private string guideCardsDeleted;
        public string GuideCardsDeleted
        {
            get { return guideCardsDeleted; }
            set { SetProperty(ref guideCardsDeleted, value); }
        }

        private string guideCardsUpdated;
        public string GuideCardsUpdated
        {
            get { return guideCardsUpdated; }
            set { SetProperty(ref guideCardsUpdated, value); }
        }


        private bool showUpdateBtn;
        public bool ShowUpdateBtn
        {
            get { return showUpdateBtn; }
            set { SetProperty(ref showUpdateBtn, value); }
        }

        private bool showOpenBtn;
        public bool ShowOpenBtn
        {
            get { return showOpenBtn; }
            set { SetProperty(ref showOpenBtn, value); }
        }

        private bool expired;
        public bool Expired
        {
            get { return expired; }
            set { SetProperty(ref expired, value); }
        }

        private bool isFTC;
        public bool IsFTC
        {
            get { return isFTC; }
            set { SetProperty(ref isFTC, value); }
        }

        private bool isTitleSelected;
        public bool IsTitleSelected
        {
            get { return isTitleSelected; }
            set { SetProperty(ref isTitleSelected, value); }
        }

        #endregion


        public PublicationViewModel()
        {
            DownloadCommand = DelegateCommand.FromAsyncHandler(async () => await Download(true));
            CancelDownloadCommand = DelegateCommand.FromAsyncHandler(CancelDownloadAction);
            ShowInfoCommand = new DelegateCommand(ShowInfo);
            DeleteCommand = DelegateCommand.FromAsyncHandler(Delete);
            GotoConentCommand = new DelegateCommand(GotoContentPage);

            ClearDownloadErrorCommand = new DelegateCommand(() =>
            {
                DownloadFailed = false;
                //ShowBookStatus = true;
                UpdateStatus();
            });

        }


        public PublicationViewModel(Publication publication)
            : this()
        {
            bookId = publication.BookId;
            UpdateData(publication);
        }


        public void UpdateData(Publication publication)
        {
            if (bookId != publication.BookId)
            {
                return;
            }
            Title = publication.ShelfViewBookTitle;
            FullTitle = publication.Name;
            Author = publication.Author;

            PrimaryColor = publication.ColorPrimary;
            SecondaryColor = publication.ColorSecondary;
            FontColor = publication.FontColor;
            IsLoan = publication.IsLoan;
            Size = publication.Size;
            SizeInfo = GetSizeLabel(Size);
            var dateFormat = ResourceLoader.GetString("DateFormat");

            DaysRemaining = publication.DaysRemaining;
            LastUpdateDate = publication.LastUpdatedDate != null ? ((DateTime)publication.LastUpdatedDate).ToString(dateFormat) : null;
            UpdateCount = publication.UpdateCount;

            PublicationStatus = publication.PublicationStatus;

            Version = publication.CurrentVersion.ToString();
            InstalledDate = publication.InstalledDate == null ? null : ((DateTime)publication.InstalledDate).ToString(dateFormat);
            CurrencyDate = publication.CurrencyDate == null ? null : ((DateTime)publication.CurrencyDate).ToString(dateFormat);

            PracticeArea = publication.PracticeArea;
            Subcategory = publication.SubCategory;

            GuideCardsAdded = publication.AddedGuideCard == null || publication.AddedGuideCard.Count == 0
                ? ResourceLoader.GetString("NoAddedGuideCardsMsg")
                : string.Join(Environment.NewLine, publication.AddedGuideCard.Select(x => x.Name));

            GuideCardsDeleted = publication.DeletedGuideCard == null || publication.DeletedGuideCard.Count == 0
                ? ResourceLoader.GetString("NoDeletedGuideCardsMsg")
                : string.Join(Environment.NewLine, publication.DeletedGuideCard.Select(x => x.Name));

            GuideCardsUpdated = publication.UpdatedGuideCard == null || publication.UpdatedGuideCard.Count == 0
                ? ResourceLoader.GetString("NoUpdatedGuideCardsMsg")
                : string.Join(Environment.NewLine, publication.UpdatedGuideCard.Select(x => x.Name));


            Description = publication.Description;
            // Description = "You may or may not want to uncheck the option to allow relocation. On your development Mac if you don’t uncheck this then the installer will find a version of your application inside your MonoDevelop solution and overwrite it rather than putting it into /Applications. This makes it appear as if the installer didn’t work. On non-development Macs this option allows the application to be overwritten even if the user has moved it after installing an older version.You may or may not want to uncheck the option to allow relocation. On your development Mac if you don’t uncheck this then the installer will find a version of your application inside your MonoDevelop solution and overwrite it rather than putting it into /Applications. This makes it appear as if the installer didn’t work. On non-development Macs this option allows the application to be overwritten even if the user has moved it after installing an older version.You may or may not want to uncheck the option to allow relocation. On your development Mac if you don’t uncheck this then the installer will find a version of your application inside your MonoDevelop solution and overwrite it rather than putting it into /Applications. This makes it appear as if the installer didn’t work. On non-development Macs this option allows the application to be overwritten even if the user has moved it after installing an older version.You may or may not want to uncheck the option to allow relocation. On your development Mac if you don’t uncheck this then the installer will find a version of your application inside your MonoDevelop solution and overwrite it rather than putting it into /Applications. This makes it appear as if the installer didn’t work. On non-development Macs this option allows the application to be overwritten even if the user has moved it after installing an older version.";
            CanShowInfo = publication.PublicationStatus != PublicationStatusEnum.NotDownloaded;
            LocalSize = publication.LocalSize;
            LocalSizeInfo = GetSizeLabel(LocalSize);
            UpdateStatus();

            IsFTC = publication.IsFTC;
        }

        private void UpdateStatus()
        {
            if (DaysRemaining >= 0)
            {
                Expired = false;
                if (!IsDownloading)
                {
                    ShowBookStatus = !DownloadFailed;
                    switch (PublicationStatus)
                    {
                        case PublicationStatusEnum.NotDownloaded:
                            {
                                ShowDownloadBtn = !DownloadFailed;
                                BookStatusLabel1 = ResourceLoader.GetString("DownloadText");
                                BookStatusLabel2 = SizeInfo;
                                ShowOpenBtn = false;
                                ShowUpdateBtn = false;
                                break;
                            }
                        case PublicationStatusEnum.RequireUpdate:
                            {
                                ShowDownloadBtn = !DownloadFailed;
                                BookStatusLabel1 = updateCount > 1
                                    ? updateCount + ResourceLoader.GetString("UpdatesAvailableText")
                                    : ResourceLoader.GetString("OneUpdateAvailableText");
                                BookStatusLabel2 = ResourceLoader.GetString("CurrencyDateText") + CurrencyDate;
                                ShowUpdateBtn = !DownloadFailed;
                                ShowOpenBtn = false;
                                break;

                            }
                        case PublicationStatusEnum.Downloaded:
                            {
                                BookStatusLabel1 = ResourceLoader.GetString("UpToDateText");
                                BookStatusLabel2 = ResourceLoader.GetString("CurrencyDateText") + CurrencyDate;
                                ShowDownloadBtn = false;
                                ShowOpenBtn = !DownloadFailed;
                                ShowUpdateBtn = false;
                                break;
                            }
                    }                 
                }
                else
                {
                    ShowBookStatus = false;
                    DownloadFailed = false;
                }
                if (IsLoan)
                {
                    ShowLoanInfo = true;
                    LoanInfo = DaysRemaining == 0 ? ResourceLoader.GetString("DueToExpireText") :
                        (DaysRemaining > 1 ? DaysRemaining + ResourceLoader.GetString("DaysRemainingText") : ResourceLoader.GetString("OneDayRemainingText"));
                }
            }
            else
            {
                ShowOpenBtn = true;
                ShowBookStatus = true;
                IsDownloading = false;
                DownloadFailed = false;
                Expired = true;
                BookStatusLabel1 = ResourceLoader.GetString("ExpiredText");
                BookStatusLabel2 = ResourceLoader.GetString("CurrencyDateText") + CurrencyDate;
            }
        }

        public async Task Download(bool checkLimitation)
        {
            IsDownloading = true;
            DownloadProgress = 0;
            //DownloadingLabel = GetSizeLabel(0) + " of " + SizeInfo;
            DownloadingLabel = "Waiting download";
            ShowBookStatus = false;
            DownloadFailed = false;
            ShowDownloadBtn = false;
            ShowUpdateBtn = false;
            tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;
            SubscribeLogoutEvent();
            DownloadTask = PublicationUtil.Instance.DownloadPublicationByBookId(BookId, token, (p, s) =>
            {
                DownloadProgress = p;
                if (p < 90)
                {
                    DownloadingLabel = GetSizeLabel(s) + " of " + SizeInfo;
                }
                else
                {
                    DownloadingLabel = "Installing";
                }

            },
            checkLimitation);



            //  Task.Run()

            var result = await DownloadTask;
            UnsubscribeLogoutEvent();
            IsDownloading = false;
            switch (result.DownloadStatus)
            {
                case DownLoadEnum.Success:
                    {
                        PublicationStatus = PublicationStatusEnum.Downloaded;
                        UpdateData(result.Publication);
                        break;

                    }

                case DownLoadEnum.OverLimitation:
                    {
                        UpdateStatus();
                        await AlertMessageService.ShowAsync(ResourceLoader.GetString("DownloadOverLimitMsg"), ResourceLoader.GetString("DownloadOverLimitTitle"),

                            new UICommand
                            {
                                Label = ResourceLoader.GetString("CancelText")
                            },

                            new UICommand
                            {
                                Label = ResourceLoader.GetString("DownloadText"),
                                Invoked = async _=>
                                {
                                    await Download(false);
                                }
                            }

                            );
                        break;
                    }
                case DownLoadEnum.Canceled:
                    {
                        DownloadFailed = true;
                        UpdateStatus();
                        DownloadFailedLabel = PublicationStatus == PublicationStatusEnum.NotDownloaded
                            ? ResourceLoader.GetString("DownloadFailedMsg")
                            : ResourceLoader.GetString("UpdateFailedMsg");
                        break;
                    }

                case DownLoadEnum.Failure:
                    {
                        DownloadFailed = true;
                        UpdateStatus();
                        DownloadFailedLabel = PublicationStatus == PublicationStatusEnum.NotDownloaded
                            ? ResourceLoader.GetString("DownloadFailedMsg")
                            : ResourceLoader.GetString("UpdateFailedMsg");
                        await AlertMessageService.ShowAsync(ResourceLoader.GetString("InstallationErrorMsg"), ResourceLoader.GetString("InstallationErrorHeader"), new UICommand(ResourceLoader.GetString("OKMsg")));
                        break;
                    }
                case DownLoadEnum.NetDisconnected:
                    {
                        DownloadFailed = true;
                        UpdateStatus();
                        DownloadFailedLabel = PublicationStatus == PublicationStatusEnum.NotDownloaded
                            ? ResourceLoader.GetString("DownloadFailedMsg")
                            : ResourceLoader.GetString("UpdateFailedMsg");
                        await AlertMessageService.ShowAsync(ResourceLoader.GetString("MissingConnectionMsg"), ResourceLoader.GetString("MissingConnectionHeader"), new UICommand(ResourceLoader.GetString("OKMsg")));
                        break;
                        //break;
                    }
            }

        }

        public async Task CancelDownloadAction()
        {

            await AlertMessageService.ShowAsync(
                 ResourceLoader.GetString("CancelDownloadMsg"),
                ResourceLoader.GetString("CancelDownloadTitle"),
                 new UICommand(
                      ResourceLoader.GetString("CancelText")),
                 new UICommand(ResourceLoader.GetString("ConfirmText"),
                      _ =>
                      {
                          CancelDownload(true);
                      }));
        }

        private void CancelDownload(bool status)
        {
            if (tokenSource != null)
            {
                tokenSource.Cancel(false);
            }
        }

        private string GetSizeLabel(long s)
        {
            if (s > 1024 * 1024)
            {
                return ((decimal)s / (1024 * 1024)).ToString("0.00") + "MB";
            }
            if (s > 1024)
            {
                return ((decimal)s / (1024)).ToString("0.00") + "KB";
            }

            return s + "B";
        }

        public void ShowInfo()
        {
            if (NavigationService != null && PublicationStatus != PublicationStatusEnum.NotDownloaded)
            {
                NavigationService.Navigate("PublicationInfo", BookId);
            }
        }


        private void SubscribeLogoutEvent()
        {
            EventAggregator.GetEvent<LogoutEvent>().Subscribe(CancelDownload, ThreadOption.BackgroundThread);
        }

        private void UnsubscribeLogoutEvent()
        {
            EventAggregator.GetEvent<LogoutEvent>().Unsubscribe(CancelDownload);
        }


        private async Task Delete()
        {
            await
                AlertMessageService.ShowAsync(
                    ResourceLoader.GetString("DeletePublicationAlertMsg"),
                    ResourceLoader.GetString("DeletePublicationAlertTitle"),
                    new UICommand(ResourceLoader.GetString("CancelText")),
                    new UICommand(ResourceLoader.GetString("ConfirmText"), async c =>
                    {
                        EventAggregator.GetEvent<PublicationDeletedEvent>().Publish(BookId);
                        await PublicationUtil.Instance.DeletePublicationByUser(BookId);
                    }));
        }

        private void GotoContentPage()
        {
            var navigationService = IoCContainer.Instance.ResolveInterface<INavigationService>();
            navigationService.Navigate("Content", BookId);
        }
    }
}
