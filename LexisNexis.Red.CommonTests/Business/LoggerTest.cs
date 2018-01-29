using LexisNexis.Red.Common;
using LexisNexis.Red.Common.HelpClass;
using LexisNexis.Red.Common.Interface;
using LexisNexis.Red.CommonTests.Impl;
using LexisNexis.Red.CommonTests.Interface;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.CommonTests.Business
{
    public class LoggerTest
    {
        [Test, Repeat(1)]
        public void Log()
        {
            string str = "DIVISION IV&#65533; &amp;";
            str = System.Web.HttpUtility.HtmlDecode(str);
            str = System.Web.HttpUtility.HtmlEncode(str);
            str = System.Web.HttpUtility.HtmlDecode(str);
            try
            {
                Logger.Log("123");
                Logger.Log("123");
                Logger.Log("123");
                Logger.Log("123");
                Logger.Log("123");
                Logger.Log("123");
                Logger.Log("123");

                Logger.Log("123");
                Logger.Log("123");
                Logger.Log("123");
                Logger.Log("123");
                Logger.Log("123");
                Logger.Log("123");
                Logger.Log("123");
                Logger.Log("123");
                Logger.Log("123");
                Logger.Log("123");
            }
            catch (Exception)
            {
                throw;
            }

        }

    }
}
