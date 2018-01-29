using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.HelpClass.Tools
{
    public interface IFileManager
    {
        Task<byte[]> ReadAllBytes(string fileName);
    }
}
