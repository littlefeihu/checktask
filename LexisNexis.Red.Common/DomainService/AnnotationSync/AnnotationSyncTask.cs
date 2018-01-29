using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Common.HelpClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.DomainService.AnnotationSync
{
    public class AnnotationSyncTask : IAnnotationSyncTask
    {

        public AnnotationSyncTask(DlBook publication)
            : this(publication, GlobalAccess.DeviceId, GlobalAccess.Instance.CurrentUserInfo)
        {

        }
        public AnnotationSyncTask(IList<DlBook> publications)
            : this(publications, GlobalAccess.DeviceId, GlobalAccess.Instance.CurrentUserInfo)
        {

        }
        public AnnotationSyncTask(string deviceId, LoginUserDetails currentUser)
        {
            Email = currentUser.Email;
            DeviceId = deviceId;
            UserSpecificFolder = currentUser.UserFolder;
            ServiceCode = currentUser.Country.ServiceCode;
            CountryCode = currentUser.Country.CountryCode;
        }

        public AnnotationSyncTask(DlBook publication, string deviceId, LoginUserDetails currentUser)
            : this(deviceId, currentUser)
        {
            //BookIds = new List<int> { publication.BookId };

            //CurrentVersion = new Dictionary<int, int>
            //{
            //    {publication.BookId, publication.CurrentVersion}
            //};
        }
        public AnnotationSyncTask(IList<DlBook> publications, string deviceId, LoginUserDetails currentUser)
            : this(deviceId, currentUser)
        {
            BookIds = new List<int>();
            CurrentVersion = new Dictionary<int, int>();
            //foreach (var dlBook in publications)
            //{
            //    if (!CurrentVersion.ContainsKey(dlBook.BookId))
            //    {
            //        CurrentVersion.Add(dlBook.BookId, dlBook.CurrentVersion);
            //        BookIds.Add(dlBook.BookId);
            //    }
            //}
        }

        public IList<int> BookIds
        {
            get;
            private set;
        }

        public string UserSpecificFolder
        {
            get;
            private set;
        }


        public string Email
        {
            get;
            private set;
        }

        public IDictionary<int, int> CurrentVersion
        {
            get;
            private set;
        }

        public string DeviceId
        {
            get;
            private set;
        }

        public string ServiceCode
        {
            get;
            private set;
        }

        public string CountryCode
        {
            get;
            private set;
        }
        public bool IsSyncTagsOnly
        {
            get
            {
                return (BookIds == null);
            }
        }
    }
}
