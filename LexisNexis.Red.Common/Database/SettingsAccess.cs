using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.Database
{
    public class SettingsAccess : DbHelper, ISettingsAccess
    {
        private string getSettingsByName = "SELECT * FROM Settings WHERE Name=? AND Email=? AND serviceCode=?";

        public Settings GetSetting(SettingsEnum settingName, string email, string serviceCode)
        {
            return base.GetEntity<Settings>(base.MainDbPath, getSettingsByName, settingName.ToString(), email, serviceCode);
        }

        public Settings GetDictionaryVersion(CountryEnum countryName, string email, string serviceCode)
        {
            return base.GetEntity<Settings>(base.MainDbPath, getSettingsByName, countryName.ToString(), email, serviceCode);
        }

        public int UpdateSetting(string value, SettingsEnum settingName, string email, string serviceCode)
        {
            var setting = GetSetting(settingName, email, serviceCode);
            if (setting == null)
            {
                setting = new Settings
                {
                    Name = settingName.ToString(),
                    Email = email,
                    Value = value,
                    ServiceCode = serviceCode
                };
                return base.Insert<Settings>(base.MainDbPath, setting);
            }
            else
            {
                setting.Value = value;
                return base.Update<Settings>(base.MainDbPath, setting);
            }
        }

        public int UpdateDictionaryVersion(string value, CountryEnum settingName, string email, string serviceCode)
        {
            var setting = GetDictionaryVersion(settingName, email, serviceCode);
            if (setting == null)
            {
                setting = new Settings
                {
                    Name = settingName.ToString(),
                    Email = email,
                    Value = value,
                    ServiceCode = serviceCode
                };
                return base.Insert<Settings>(base.MainDbPath, setting);
            }
            else
            {
                setting.Value = value;
                return base.Update<Settings>(base.MainDbPath, setting);
            }
        }
    }
}
