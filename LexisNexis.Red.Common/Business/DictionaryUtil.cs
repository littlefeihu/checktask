using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.IO;
using System.Linq;
using SQLite;
using LexisNexis.Red.Common;
using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Common.Common;
using LexisNexis.Red.Common.HelpClass;
using LexisNexis.Red.Common.Segment;
using System.Threading.Tasks;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Database;
using LexisNexis.Red.Common.DominService;
using LexisNexis.Red.Common.Interface;
using System.Threading;
using LexisNexis.Red.Common.HelpClass.DomainEvent;
using LexisNexis.Red.Common.DomainService.Events;

namespace LexisNexis.Red.Common.Business
{
    /// <summary>
    /// Search Service
    /// </summary>
    public class DictionaryUtil
    {
        private static string defaultVersion = "0.0";
        private static int isDownloading = 0;
        //private static int AUTO_RETRY_TIMES = 5;
        private static IDictionaryAccess dictionaryAccess = IoCContainer.Instance.ResolveInterface<IDictionaryAccess>();
        private static SettingsAccess settingsAccess = IoCContainer.Instance.ResolveInterface<SettingsAccess>();
        private static IPublicationAccess publicationAccess = IoCContainer.Instance.ResolveInterface<IPublicationAccess>();
        private static IDictionaryDomainService dictionaryDomainService = IoCContainer.Instance.ResolveInterface<IDictionaryDomainService>();
        //private static IConnectionMonitor connectionMonitor = new ConnectionMonitor();
        //private static SemaphoreSlim _syncLock = new SemaphoreSlim(1);

        public static string[] PUNCTUATION_LIST = {"=",",",".",";","+","-","|","/",
        "\\","\"",":","?","<",">","[","]","{","}","!","@","#","$","%","^","&","*","(",")","~","`",
        "　","，","。","；","‘","’","“","”","／","？","～","！","＠","＃","￥","％","……","＆","×","（","—）",
        "【","】","｛","｝","｜","、","《","》","：","_","\r\n","—"};

        /// <summary>
        /// UpdateDictionary
        /// </summary>
        /// <returns></returns>
        public static Task UpdateDictionary(string countryCode)
        {
            return Task.Run(async () =>
            {
                try
                {
                    if (Interlocked.CompareExchange(ref isDownloading, 1, 0) == 0)
                    {
                        //await _syncLock.WaitAsync(); 
                        isDownloading = 1;
                        bool needUpdate = false;
                        string dictionaryPath = Path.Combine(GlobalAccess.DirectoryService.GetAppRootPath(), Constants.DICITIONARY_DB_NAME + countryCode + Constants.DICITIONARY_EXTENSION_NAME);
                        DictionaryVersionResponse version = await dictionaryDomainService.GetDictionaryVersion();
                        Dictionary<String, String> versionPool = new Dictionary<string, string>();
                        versionPool.Add(CountryEnum.AU.ToString(), version.AU ?? defaultVersion);
                        versionPool.Add(CountryEnum.NZ.ToString(), version.NZ ?? defaultVersion);
                        versionPool.Add(CountryEnum.SG.ToString(), version.SG ?? defaultVersion);
                        versionPool.Add(CountryEnum.HK.ToString(), version.HK ?? defaultVersion);
                        versionPool.Add(CountryEnum.JP.ToString(), version.JP ?? defaultVersion);
                        versionPool.Add(CountryEnum.MY.ToString(), version.MY ?? defaultVersion);
                        CountryEnum dictionaryLocation = (CountryEnum)Enum.Parse(typeof(CountryEnum), countryCode);
                        string currentDictionaryVersion = defaultVersion;
                        double versionDouble = 0.0;
                        bool isVersionDouble = Double.TryParse(versionPool[countryCode], out versionDouble);
                        if (versionPool[countryCode].Equals(defaultVersion) || !isVersionDouble || String.IsNullOrEmpty(GlobalAccess.Instance.CurrentUserInfo.Country.DictionaryUrl))
                        {
                            needUpdate = false;
                        }
                        else if (!await GlobalAccess.DirectoryService.FileExists(dictionaryPath))
                        {
                            needUpdate = true;
                        }
                        else
                        {
                            Settings st = null;
                            string versionLocal = String.Empty;
                            st = settingsAccess.GetDictionaryVersion(dictionaryLocation, Constants.GLOBAL_SETTING_MAIL, Constants.GLOBAL_SETTING_COUNTRY_CODE);
                            if (st != null)
                            {
                                versionLocal = st.Value;
                                double versionLocalDouble = 0.0;
                                bool isversionLocalDouble = Double.TryParse(versionLocal, out versionLocalDouble);
                                if (isversionLocalDouble && (String.IsNullOrEmpty(versionLocal.Trim()) || versionDouble > versionLocalDouble))
                                {
                                    needUpdate = true;
                                }
                            }
                            else
                            {
                                needUpdate = true;
                            }
                        }
                        if (needUpdate)
                        {
                            try
                            {
                                currentDictionaryVersion = versionPool[countryCode];
                                await dictionaryDomainService.DownloadDictionary(GlobalAccess.Instance.CurrentUserInfo.Country.DictionaryUrl, Constants.DICITIONARY_DB_NAME + countryCode + Constants.DICITIONARY_EXTENSION_NAME);
                                settingsAccess.UpdateDictionaryVersion(currentDictionaryVersion, dictionaryLocation, Constants.GLOBAL_SETTING_MAIL, Constants.GLOBAL_SETTING_COUNTRY_CODE);
                            }
                            catch (Exception e)
                            {
                                isDownloading = 0;
                                settingsAccess.UpdateDictionaryVersion(defaultVersion, dictionaryLocation, Constants.GLOBAL_SETTING_MAIL, Constants.GLOBAL_SETTING_COUNTRY_CODE);
                                Logger.Log("Download Dictionary Failed : " + e.Message);
                                //if (AUTO_RETRY_TIMES >= 0)
                                //{
                                //    AUTO_RETRY_TIMES--;
                                //    Logger.Log("Download Dictionary Failed : " + e.Message + " Retry : " + (6 - AUTO_RETRY_TIMES).ToString());
                                //    UpdateDictionary();
                                //}
                            }
                        }
                        //AUTO_RETRY_TIMES = 5;
                        isDownloading = 0;
                    }
                }
                catch (Exception ex)
                {
                    isDownloading = 0;
                    Logger.Log("Update Dictionary Failed : " + ex.Message);
                    //if (AUTO_RETRY_TIMES >= 0)
                    //{
                    //    AUTO_RETRY_TIMES--;
                    //    Logger.Log("Update Dictionary Failed : " + ex.Message + " Retry : " + (6 - AUTO_RETRY_TIMES).ToString());
                    //    UpdateDictionary();
                    //}
                }
                finally
                {
                    //_syncLock.Release();
                }
            });
        }

        /// <summary>
        /// SearchDictionary
        /// </summary>
        /// <param name="term"></param>
        /// <returns></returns>
        public static LegalDefinitionItem SearchDictionary(string term, int bookID)
        {
            LegalDefinitionItem result = new LegalDefinitionItem();
            string countryCode = publicationAccess.GetDlBookByBookId(bookID, GlobalAccess.Instance.UserCredential).CountryCode.ToUpper();
            string dictionaryPath = Path.Combine(GlobalAccess.DirectoryService.GetAppExternalRootPath(), Constants.DICITIONARY_DB_NAME + countryCode + Constants.DICITIONARY_EXTENSION_NAME);
            try
            {
                //bool pingResult = await connectionMonitor.PingService(GlobalAccess.Instance.CountryCode);

                //if (pingResult && DictionaryUtil.IsDictionaryDownloaded())
                //{
                //    DictionaryUtil.UpdateDictionary().Wait();
                //}

                if (!String.IsNullOrEmpty(term.Trim()))
                {
                    string keyword = term.Trim();
                    result = dictionaryAccess.SearchDictionary(keyword, countryCode);
                    if (result.ContextualDefinitions.Count == 0)
                    {
                        Regex replaceSpace = new Regex(@"\s{2,}", RegexOptions.IgnoreCase);
                        foreach (string punctuation in PUNCTUATION_LIST)
                        {
                            term = term.Replace(punctuation, Constants.SPACE);
                        }
                        keyword = replaceSpace.Replace(StringUtil.ToSingular(term.ToLower().Trim()), Constants.SPACE);
                        result = dictionaryAccess.SearchDictionary(keyword, countryCode);
                    }
                    if (result.ContextualDefinitions.Count == 0)
                    {
                        result = dictionaryAccess.SearchDictionary(StringUtil.ToSingular(StringUtil.RemoveApostrophes(keyword)), countryCode);
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Logger.Log("Search Dictionary Failed : " + ex.Message);
                return result;
            }
        }

        /// <summary>
        /// IsDictionaryDownloaded
        /// </summary>
        /// <returns></returns>
        public static bool IsDictionaryDownloaded(int bookId)
        {
            bool result = false;
            string countryCode = publicationAccess.GetDlBookByBookId(bookId, GlobalAccess.Instance.UserCredential).CountryCode.ToUpper();
            string dictionaryPath = Constants.DICITIONARY_DB_NAME + countryCode + Constants.DICITIONARY_EXTENSION_NAME;
            if (IsDictionaryExisted(dictionaryPath).Result)
            {
                result = true;
            }
            return result;
        }

        private static Task<bool> IsDictionaryExisted(string dictionaryPath)
        {
            return Task.Run(async () =>
            {
                return await GlobalAccess.DirectoryService.FileExists(dictionaryPath);
            });
        }
    }
}