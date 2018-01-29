using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.HelpClass.DomainEvent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.DomainService.Events
{
    public class LoginSuccessEvent : IEvent
    {

        public LoginSuccessEvent(LoginUserDetails loginUserDetails)
        {
            LoginUserDetails = loginUserDetails;
        }
        public LoginUserDetails LoginUserDetails
        {
            get;
            private set;
        }
    }
}
