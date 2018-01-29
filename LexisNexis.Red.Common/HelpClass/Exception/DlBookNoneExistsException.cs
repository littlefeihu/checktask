using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.HelpClass
{
    public class DlBookNoneExistsException : Exception
    {
        public DlBookNoneExistsException()
        {

        }
        public DlBookNoneExistsException(string message)
            : base(message)
        {

        }
    }
}
