using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Common;
using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Common.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.HelpClass.Tools
{
    public class DecryptManager
    {
        private static ICryptogram cryptogram = IoCContainer.Instance.ResolveInterface<ICryptogram>();

        public static async Task<string> DecryptContentFromZipFile(string fileName, byte[] bookKey, byte[] initVector)
        {
            // get bytes from encrypted zip 
            Byte[] fileContent = await FileManager.ReadAllBytes(fileName);
            if (fileContent.Length == 0)
                return string.Empty;
            // get zip bytes from encrypted zip 
            Byte[] bufferBytes = await cryptogram.DecryptBytes(fileContent, bookKey, initVector);

            var unzipBytes = await IoCContainer.Instance.ResolveInterface<IPackageFile>().UnZipAsync(bufferBytes);

            return Encoding.UTF8.GetString(unzipBytes, 0, unzipBytes.Length);
        }

        public static Task DecryptDataBase(string originFileName, string targetFileName, byte[] contentKey, byte[] initVector)
        {
            return Task.Run(async () =>
            {
                Byte[] fileContent = await FileManager.ReadAllBytes(originFileName);
                if (fileContent.Length == 0)
                    return;
                Byte[] bufferBytes = await cryptogram.DecryptBytes(fileContent, contentKey, initVector);
                using (Stream outPutStream = await GlobalAccess.DirectoryService.OpenFile(targetFileName, FileModeEnum.Create))
                {
                    await outPutStream.WriteAsync(bufferBytes, 0, bufferBytes.Length);
                    await outPutStream.FlushAsync();
                }
            });

        }
    }
}
