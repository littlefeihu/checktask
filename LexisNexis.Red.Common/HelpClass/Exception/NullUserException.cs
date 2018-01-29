using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.HelpClass
{
    public class NullUserException : Exception
    {
        public NullUserException()
        {

        }
        public NullUserException(string message)
            : base(message)
        {

        }
    }
}
