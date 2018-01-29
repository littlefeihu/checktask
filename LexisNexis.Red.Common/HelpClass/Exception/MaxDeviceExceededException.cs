using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.HelpClass
{
    public class MaxDeviceExceededException : Exception
    {
        public MaxDeviceExceededException()
        {

        }
        public MaxDeviceExceededException(string message)
            : base(message)
        {

        }
    }
}
