using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.Interface
{
    public interface IPackageFile
    {
        /// <summary>
        /// Extract file to required path
        /// </summary>
        /// <param name="sourceFileName">zip name </param>
        /// <param name="targetPath">target path</param>
        /// <param name="cancelToken">cancelToken</param>
        /// <returns></returns>
        Task DepressFile(string sourceFileName, string targetPath, CancellationToken cancelToken);
        /// <summary>
        ///  unzip 
        /// </summary>
        /// <param name="bytes">zip bytes data</param>
        /// <param name="cancelToken">cancelToken</param>
        /// <returns>unzip bytes data</returns>
        Task<Byte[]> UnZipAsync(Byte[] bytes);

    }
}
