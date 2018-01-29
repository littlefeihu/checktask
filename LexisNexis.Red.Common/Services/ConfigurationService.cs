using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using LexisNexis.Red.Common.BusinessModel;
using System.Xml.Serialization;
using System.Linq;
using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Common.HelpClass;
using LexisNexis.Red.Common.Common;

namespace LexisNexis.Red.Common.Services
{
    public static class ConfigurationService
    {
        #region private member
        private static string resourceName;
        private static List<Country> country = null;
        #endregion

        #region public method
        /// <summary>
        /// getallcountrymap
        /// </summary>
        /// <returns></returns>
        public static List<Country> GetAllCountryMap()
        {
            return country;
        }
        #endregion

        internal static void GetConfigurations()
        {
#if PREVIEW
            resourceName = "LexisNexis.Red.Common.Config.Preview.xml";
#elif RELEASE
            resourceName = "LexisNexis.Red.Common.Config.Release.xml";
#elif TESTING
            resourceName = "LexisNexis.Red.Common.Config.Testing.xml";
#else
            resourceName = "LexisNexis.Red.Common.Config.Development.xml";
#endif
            #region loadcountrymap
            using (var stream = ResouceHelper.GetStreamFromAssembly(Constants.ASSEMBLY_NAME, resourceName))
            {
                var serializer = new XmlSerializer(typeof(List<Country>));
                var list = serializer.Deserialize(stream) as List<Country>;
                if (list != null)
                {
                    country = list.OrderBy(o => o.CountryName).ToList();
                }
                else
                {
                    throw new Exception("CountryMapping.xml Deserialized fail");
                }
            }
            #endregion
        }
    }

}
