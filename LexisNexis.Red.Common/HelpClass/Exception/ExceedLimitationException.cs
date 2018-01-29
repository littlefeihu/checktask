using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.HelpClass
{
    public class ExceedLimitationException : Exception
    {
        public ExceedLimitationException()
        {

        }
        public ExceedLimitationException(string message)
            : base(message)
        {

        }
    }
}
