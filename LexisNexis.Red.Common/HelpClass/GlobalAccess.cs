using System.IO;
using System.Reflection;
using LexisNexis.Red.Common.Common;
using LexisNexis.Red.Common.Interface;
using LexisNexis.Red.Common.Services;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Database;
using System.Collections.Generic;
using System.Threading.Tasks;
using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Common.HelpClass.Tools;
using System;
using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.DomainService;
using System.Linq;
using LexisNexis.Red.Common.Segment;
using LexisNexis.Red.Common.DominService;
using System.Threading;
namespace LexisNexis.Red.Common.HelpClass
{
    public class GlobalAccess
    {
        private static readonly GlobalAccess instance = new GlobalAccess();
        private static string deviceid;
        private static string deviceos;
        private LoginUserDetails loginUserDetails;
        private UserCredential userCredential;

        private GlobalAccess()
        {
        }
        static GlobalAccess()
        {

        }
        public Task Init()
        {
            return Task.Run(async () =>
            {
                ConfigurationService.GetConfigurations();
                Bootstrapper.Init();
                await InitializeDatabase();
                await InitializeSegment();
            });
        }
        #region singletonproperty
        public static GlobalAccess Instance
        {
            get
            {
                return instance;
            }
        }
        #endregion


        public static IDevice DeviceService
        {

            get
            {
                return IoCContainer.Instance.ResolveInterface<IDevice>();
            }
        }

        public static IDirectory DirectoryService
        {
            get
            {
                return IoCContainer.Instance.ResolveInterface<IDirectory>();
            }
        }

        public ISegment SegmentService { get; private set; }

        public static string DeviceOS
        {
            get
            {
                if (string.IsNullOrEmpty(deviceos))
                {
                    deviceos = DeviceService.GetDeviceOS();
                }

                return deviceos;
            }

        }
        public static string DeviceId
        {
            get
            {
                if (string.IsNullOrEmpty(deviceid))
                {
                    deviceid = DeviceService.GetDeviceID();
                }

                return deviceid;
            }

        }

        public LoginUserDetails CurrentUserInfo
        {
            get
            {
                if (loginUserDetails == null)
                    loginUserDetails = LoginUtil.Instance.GetLastIsAliveLoginUser();
                return loginUserDetails;
            }
            internal set
            {
                loginUserDetails = value;
            }
        }

        public UserCredential UserCredential
        {
            get
            {
                if (CurrentUserInfo != null)
                    userCredential = new UserCredential(CurrentUserInfo.Email, CurrentUserInfo.Country.ServiceCode);
                else
                {
                    userCredential = null;
                }
                return userCredential;
            }
        }

        public string Email
        {
            get
            {
                if (CurrentUserInfo != null)
                    return CurrentUserInfo.Email;
                else
                    return null;
            }
        }

        public string CountryCode
        {
            get
            {
                if (CurrentUserInfo != null)
                    return CurrentUserInfo.Country.CountryCode;
                else
                    return null;
            }
        }
        public string ServiceCode
        {
            get
            {
                if (CurrentUserInfo != null)
                    return CurrentUserInfo.Country.ServiceCode;
                else
                    return null;
            }
        }
        public PublicationContent CurrentPublication { get; set; }

        private async Task InitializeDatabase()
        {
#if DEBUG
            if (!await DirectoryService.DirectoryExists(DirectoryService.GetAppRootPath()))
            {
                await DirectoryService.CreateDirectory(DirectoryService.GetAppRootPath());
            }
#endif
            if (!await DirectoryService.InternalFileExists(Constants.DATABASE_FILE_NAME))
            {
                using (var stream = ResouceHelper.GetStreamFromAssembly(Constants.ASSEMBLY_NAME, Constants.INITIAL_DATABASE))
                {
                    using (var ms = new MemoryStream())
                    {
                        await stream.CopyToAsync(ms);
                        await DirectoryService.SaveFileToInternal(Constants.DATABASE_FILE_NAME, ms.ToArray());
                    }
                }
            }
        }

        private async Task InitializeSegment()
        {
            try
            {
                await SegmentUtil.Instance.Prepare();
            }
            catch (Exception ex)
            {
                Logger.Log("Initialize Dictionary Failed : " + ex.Message);
                return;
            }
        }

    }
}
