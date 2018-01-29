using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.HelpClass
{
    public class NullDirectoryException : Exception
    {
        public NullDirectoryException()
        {

        }
        public NullDirectoryException(string message)
            : base(message)
        {

        }
    }
}
