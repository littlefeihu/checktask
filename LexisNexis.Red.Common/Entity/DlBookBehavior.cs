using LexisNexis.Red.Common.HelpClass;
using LexisNexis.Red.Common.Interface;
using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using LexisNexis.Red.Common.Common;
using System.Threading;
using LexisNexis.Red.Common.HelpClass.Tools;
using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.BusinessModel;
namespace LexisNexis.Red.Common.Entity
{
    public partial class DlBook
    {
        public void SetDlBookExpired()
        {
            bool hasNotExpired = (ValidTo.Value.DaysRemaining() >= 0);
            if (hasNotExpired)
            {
                ValidTo = DateTime.Now.AddDays(-1).Date;
            }
        }
        public DlBook UpdateFromServer(DlBook serverDlBook, bool requireUpdateKey = false)
        {
            if (DlStatus == (short)DlStatusEnum.NotDownloaded || requireUpdateKey)
            {
                InitVector = serverDlBook.InitVector;
                K2Key = serverDlBook.K2Key;
                HmacKey = serverDlBook.HmacKey;

                ColorPrimary = serverDlBook.ColorPrimary;
                ColorSecondary = serverDlBook.ColorSecondary;
                FontColor = serverDlBook.FontColor;

                Name = serverDlBook.Name;
                Author = serverDlBook.Author;
                Description = serverDlBook.Description;
                DpsiCode = serverDlBook.DpsiCode;
            }

            LastUpdatedDate = serverDlBook.LastUpdatedDate;
            FileUrl = serverDlBook.FileUrl;
            Size = serverDlBook.Size;
            CurrentVersion = serverDlBook.CurrentVersion;

            IsLoan = serverDlBook.IsLoan;
            IsTrial = serverDlBook.IsTrial;
            ValidTo = serverDlBook.ValidTo;
            return this;
        }


        public async Task<byte[]> GetContentKey(string userKey)
        {
            var ivBytes = Convert.FromBase64String(InitVector);
            var key3 = Convert.FromBase64String(K2Key);
            var k1 = Convert.FromBase64String(userKey);
            var cryptogram = IoCContainer.Instance.ResolveInterface<ICryptogram>();
            string decryptKey2String = await cryptogram.DecryptStringFromBytesAes((byte[])key3, k1, (byte[])ivBytes);

            var key2 = Convert.FromBase64String(decryptKey2String);
            var hmac = Convert.FromBase64String(HmacKey);
            var decryptedHmac = cryptogram.GenerateHMAC(k1, key2);
            if (decryptedHmac.SequenceEqual((byte[])hmac))
            {
                return key2;
            }
            else
            {
                return null;
            }
        }

        public string UserFolder()
        {
            return Path.Combine(ServiceCode, Email);
        }

        public string GetDirectory()
        {
            var combinedpath = Path.Combine(UserFolder(), Constants.DLFiles, BookId + "V" + LastDownloadedVersion);
            return combinedpath;
        }
        public string GetDlBookZipFileName()
        {
            return Path.Combine(UserFolder(), BookId + Constants.ZIP);
        }
        public string GetDecryptedDbFullName()
        {
            return GetDbPath(Constants.SQLITE_Decrypted);
        }

        public string GetIndexDbFullName()
        {
            return GetDbPath(Constants.INDEX_DB_PATH);
        }

        public string GetXpathDbFullName()
        {
            return GetDbPath(Constants.SQLITE_Xpath);
        }
        private string GetDbPath(string dbName)
        {
            return Path.Combine(GlobalAccess.DirectoryService.GetAppExternalRootPath(), GetDlBookSqliteFilename(dbName));
        }

        public string GetDlBookSqliteFilename(string filename)
        {
            return Path.Combine(GetDirectory(), filename);
        }
        public async Task<bool> IsSqliteDecryped()
        {
            var decrypedFullName = GetDlBookSqliteFilename(Constants.SQLITE_Decrypted);
            return await GlobalAccess.DirectoryService.FileExists(decrypedFullName);
        }
        public Task DeleteDlBookInstallFile()
        {
            return Task.Run(async () =>
              {
                  string path = GetDirectory();
                  if (!string.IsNullOrEmpty(path))
                  {
                      if (await GlobalAccess.DirectoryService.DirectoryExists(path))
                      {
                          await GlobalAccess.DirectoryService.DeleteDirectory(path);
                      }
                  }
              });
        }
        public async Task UnZipFile(CancellationToken cancelToken)
        {
            string sourceFileName = GetDlBookZipFileName();
            if (await GlobalAccess.DirectoryService.FileExists(sourceFileName))
            {
                string targetDirectory = GetDirectory();
                if (!await GlobalAccess.DirectoryService.DirectoryExists(targetDirectory))
                {
                    await GlobalAccess.DirectoryService.CreateDirectory(targetDirectory);
                }
                await IoCContainer.Instance.ResolveInterface<IPackageFile>().DepressFile(sourceFileName, targetDirectory, cancelToken);
            }

        }

        public byte[] GetInitVector()
        {
            return Convert.FromBase64String(this.InitVector);
        }

        public async Task DecryptDataBase(byte[] contentKey)
        {
            string databasePath = GetDlBookSqliteFilename(BookId + Constants.SQLITE);
            string decryptDatabasePath = GetDlBookSqliteFilename(Constants.SQLITE_Decrypted);
            if (!await GlobalAccess.DirectoryService.FileExists(decryptDatabasePath))
            {
                await DecryptManager.DecryptDataBase(databasePath, decryptDatabasePath, contentKey, GetInitVector());
            }
        }

        public async Task DeleteOriginalSqlite()
        {
            var originalFullName = GetDlBookSqliteFilename(BookId + Constants.SQLITE);
            if (await GlobalAccess.DirectoryService.FileExists(originalFullName))
            {
                await GlobalAccess.DirectoryService.DeleteFile(originalFullName);
            }
        }

        public async Task DeleteDlBookZipFile()
        {
            string fileName = GetDlBookZipFileName();
            if (await GlobalAccess.DirectoryService.FileExists(fileName))
            {
                await GlobalAccess.DirectoryService.DeleteFile(fileName);
            }
        }
        public async Task<Stream> CreateDlBookFile()
        {
            string fileName = GetDlBookZipFileName();
            if (await GlobalAccess.DirectoryService.FileExists(fileName))
            {
                await DeleteDlBookZipFile();
            }
            return await GlobalAccess.DirectoryService.OpenFile(fileName);
        }

    }

}
