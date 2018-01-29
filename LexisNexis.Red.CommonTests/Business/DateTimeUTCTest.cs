using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.CommonTests.Business
{
    public class DateTimeUTCTest
    {
        [Test()]
        public void DateTimeConvert()
        {

            //DateTime date = DateTime.Now.ToLocalTime();
            DateTime date1 = DateTime.UtcNow;
            //DateTime date2 = DateTime.Now.ToUniversalTime();
            DateTime date3 = DateTime.Now;
            DateTime date4 = date1.ToLocalTime();
            DateTime date5 = date3.ToLocalTime();

            var ticks = DateTime.Now.Ticks;
            var dt = new DateTime(ticks);
            var ddd = DateTime.UtcNow.AddHours(-48).Ticks;
            var dd = DateTime.UtcNow.Subtract(DateTime.UtcNow.AddHours(-8)).TotalHours;

        }
    }
}
