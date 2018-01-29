using LexisNexis.Red.Common.Database;
using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Common.HelpClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.Business
{
    public class SettingsUtil : AppServiceBase<SettingsUtil>
    {
        IUserAccess userAccess;
        ISettingsAccess settingAccess;
        public SettingsUtil(IUserAccess userAccess, ISettingsAccess settingAccess)
        {
            this.userAccess = userAccess;
            this.settingAccess = settingAccess;
        }

        public DateTime GetLastSyncedTime()
        {
            return DateTime.Now;
        }

        public string GetTermsAndConditions(string style = null)
        {
            var terms = Properties.Resources.ResourceManager.GetString("Terms");
            return terms.Replace("<!--#Style#-->", style == null ? "" : style);
        }
        public string GetLexisNexisRedInfo(string style = null)
        {
            var redInfo = Properties.Resources.ResourceManager.GetString("LNRedInfo");
            return redInfo.Replace("<!--#Style#-->", style == null ? "" : style);
        }
        public string GetLexisNexisInfo(string style = null)
        {
            var lnInfo = Properties.Resources.ResourceManager.GetString("LNInfo");
            return lnInfo.Replace("<!--#Style#-->", style == null ? "" : style);
        }

        public string GetExpiryNotification()
        {
            return Properties.Resources.ResourceManager.GetString(GlobalAccess.Instance.CountryCode + "_ExpiryNotification");
        }
        public float GetFontSize()
        {
            var setting = settingAccess.GetSetting(BusinessModel.SettingsEnum.FontSize, GlobalAccess.Instance.Email, GlobalAccess.Instance.CountryCode);

            return setting == null ? 0 : float.Parse(setting.Value);
        }

        public int SaveFontSize(float fontSize)
        {
            return settingAccess.UpdateSetting(fontSize.ToString(), BusinessModel.SettingsEnum.FontSize, GlobalAccess.Instance.Email, GlobalAccess.Instance.CountryCode);
        }
        public void UpdateLastSyncedTime()
        {
            if (GlobalAccess.Instance.CurrentUserInfo != null)//first login current user is null
            {
                GlobalAccess.Instance.CurrentUserInfo.LastSyncDate = DateTime.Now;
                userAccess.UpdateUserLastSyncDate(GlobalAccess.Instance.Email, GlobalAccess.Instance.CountryCode);
            }
        }
    }
}
