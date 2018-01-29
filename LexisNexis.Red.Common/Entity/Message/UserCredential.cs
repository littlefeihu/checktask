using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.Entity
{
    public class UserCredential
    {
        public UserCredential(string email, string serviceCode)
        {
            this.Email = email;
            this.ServiceCode = serviceCode;
        }
        public string Email { get; private set; }


        public string ServiceCode { get; private set; }
    }
}
