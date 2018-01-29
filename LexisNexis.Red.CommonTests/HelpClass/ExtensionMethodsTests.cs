using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LexisNexis.Red.Common.HelpClass;
using NUnit.Framework;
namespace LexisNexis.Red.Common.HelpClass.Tests
{
    [TestFixture()]
    public class ExtensionMethodsTests
    {
        [Test()]
        public void ToDayMonthYearDateTest()
        {
            var dt = DateTime.Now.ToDayMonthYearDate();
         
        }
    }
}
