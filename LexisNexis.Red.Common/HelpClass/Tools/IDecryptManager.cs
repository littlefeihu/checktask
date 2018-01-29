using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.HelpClass.Tools
{
    public interface IDecryptManager
    {

        Task<string> DecryptContentFromZipFile(string fileName, byte[] bookKey, byte[] initVector);

        Task DecryptDataBase(string fileName, string targetDbFileName, byte[] bookKey, byte[] initVector);
    }
}
