using LexisNexis.Red.Common.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LexisNexis.Red.CommonTests.Impl
{
    public class PackageFile : IPackageFile
    {

        public async Task DepressFile(string sourceFileName, string targetPath, CancellationToken cancelToken)
        {
            await Task.Run(() =>
                {
                    string root = System.Configuration.ConfigurationManager.AppSettings["AppRootPath"];
                    sourceFileName = Path.Combine(root, sourceFileName);
                    targetPath = Path.Combine(root, targetPath);
                    ICSharpCode.SharpZipLib.Zip.FastZip fZip = new ICSharpCode.SharpZipLib.Zip.FastZip();
                    fZip.ExtractZip(sourceFileName, targetPath, "");
                });
        }


        public async Task<byte[]> UnZipAsync(byte[] bytes)
        {
            // get content from zip
            using (MemoryStream memStream = new MemoryStream())
            {
                string entryName = string.Empty;
                using (MemoryStream strm = new MemoryStream(bytes))
                using (ZipArchive input = new ZipArchive(strm))
                {
                    Byte[] buffer = new byte[4096];
                    if (input.Entries.Count > 0)
                    {
                        ZipArchiveEntry e = input.Entries[0];
                        entryName = e.Name;
                        using (Stream entryStream = e.Open())
                        {
                            int n = 0;
                            while ((n = await entryStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                            {
                                await entryStream.FlushAsync();
                                await memStream.WriteAsync(buffer, 0, n);
                                await memStream.FlushAsync();
                            }
                        }
                    }
                }
                return memStream.ToArray();
            }


        }
    }
}
