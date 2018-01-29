using LexisNexis.Red.Common.HelpClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.Services
{
    public class ServiceBase
    {
        string serviceSuffix = null;
        protected ServiceBase(string serviceSuffix)
        {
            this.serviceSuffix = serviceSuffix;
        }

        protected Uri GetTargetUri(string countryCode = null)
        {
            countryCode = countryCode ??  GlobalAccess.Instance.CurrentUserInfo.Country.CountryCode;
            var country = ConfigurationService.GetAllCountryMap().FirstOrDefault(o => o.CountryCode == countryCode);
            return new Uri(country.RemoteUrl + serviceSuffix);
        }
    }
}
