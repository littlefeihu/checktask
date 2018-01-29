using System;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using LexisNexis.Red.Common.Interface;

namespace LexisNexis.Red.WindowsStore.Implementation
{
    public class ZipFileManager : IPackageFile
    {
        private readonly StorageFolder localFolder = ApplicationData.Current.LocalFolder;
        public async Task DepressFile(string sourceFileName, string targetPath, CancellationToken cancelToken)
        {
            var destinationFolder = await localFolder.GetFolderAsync(targetPath);
           
            using (var stream = await localFolder.OpenStreamForReadAsync(sourceFileName))
            {
                using (var archive = new ZipArchive(stream, ZipArchiveMode.Read))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        cancelToken.ThrowIfCancellationRequested();

                        if (entry.Name != "")
                        {
                             // File
                            await ExtractFile(destinationFolder, entry);
                        }
                        else
                        {
                            // Folder
                            await CreateRecursiveFolder(destinationFolder, entry);
                           
                        }
                    }
                }
            }
        }


        private async Task CreateRecursiveFolder(StorageFolder folder, ZipArchiveEntry entry)
        {
            await folder.CreateFolderAsync(entry.FullName.Replace("/","\\"), CreationCollisionOption.OpenIfExists);
        }

        private async Task ExtractFile(StorageFolder folder, ZipArchiveEntry entry)
        {
            using (Stream fileData = entry.Open())
            {
                StorageFile outputFile = await folder.CreateFileAsync(entry.FullName.Replace("/", "\\"), CreationCollisionOption.ReplaceExisting);
                using (Stream outputFileStream = await outputFile.OpenStreamForWriteAsync())
                {
                    fileData.CopyTo(outputFileStream);
                }
            }
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
