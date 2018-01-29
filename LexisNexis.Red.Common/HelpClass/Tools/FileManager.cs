using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Common;
using LexisNexis.Red.Common.Database;
using LexisNexis.Red.Common.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LexisNexis.Red.Common.HelpClass.Tools
{
    public class FileManager
    {
        private static IDirectory directoryService = GlobalAccess.DirectoryService;
        public static async Task<byte[]> ReadAllBytes(string fileName)
        {
            using (Stream fileStream = await directoryService.OpenFile(fileName, FileModeEnum.Open))
            using (MemoryStream ms = new MemoryStream())
            {
                byte[] fileBytes = new byte[4096];
                int n = 0;
                while ((n = await fileStream.ReadAsync(fileBytes, 0, fileBytes.Length)) > 0)
                {
                    await fileStream.FlushAsync();

                    await ms.WriteAsync(fileBytes, 0, n);
                    await ms.FlushAsync();
                }
                return ms.ToArray();
            }
        }

    }
}
