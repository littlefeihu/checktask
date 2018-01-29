using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.CommonTests.Business
{
    public class TestHelper
    {
        public static List<LoginInfo> TestUsers { get; set; }

    }

    public class LoginInfo
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string CountryCode { get; set; }
    }
}
