using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.Database
{
    public interface ISettingsAccess
    {
        Settings GetSetting(SettingsEnum settingName, string email, string serviceCode);

        int UpdateSetting(string value, SettingsEnum settingName, string email, string serviceCode);

    }
}
