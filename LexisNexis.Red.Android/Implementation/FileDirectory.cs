using System;
using LexisNexis.Red.Common;
using System.IO;
using System.Threading.Tasks;
using LexisNexis.Red.Droid.App;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Droid.Utility;

namespace LexisNexis.Red.Droid.Implementation
{
    public class FileDirectory : IDirectory
    {

        private static string appLocalStorage;
        private static string appExternalStorage;

        public static string AppLocalStorage
        {
            get
            {
                return appLocalStorage;
            }
        }

        public static string AppExternalStorage
        {
            get
            {
                return appExternalStorage;
            }
        }

        public static void Init(string localStorage, string externalStorage)
        {
            appLocalStorage = localStorage;
            appExternalStorage = externalStorage;
        }

        #region IDirectory implementation

        // Internal
        public string GetAppRootPath()
        {
            return appLocalStorage;
        }

        public Task<bool> InternalFileExists(string fileName)
        {
            return Task.Run<bool>(delegate
            {
                return File.Exists(Path.Combine(appLocalStorage, fileName));
            });
        }

        public Task SaveFileToInternal(string fileName, byte[] buffer)
        {
            return Task.Run(delegate
            {
                using (var fs = new FileStream(Path.Combine(appLocalStorage, fileName), FileMode.Append))
                {
                    fs.Write(buffer, 0, buffer.Length);
                }
            });
        }

        // External
        public string GetAppExternalRootPath()
        {
            return appExternalStorage;
        }

        public Task<bool> CreateDirectory(string path)
        {
            return Task.Run<bool>(delegate
            {
                try
                {
                    Directory.CreateDirectory(Path.Combine(appExternalStorage, path));
                    return true;
                }
                catch
                {
                    return false;
                }
            });
        }

        public Task<bool> FileExists(string fileName)
        {
            return Task.Run<bool>(delegate
            {
                return File.Exists(Path.Combine(appExternalStorage, fileName));
            });
        }

        public Task DeleteFile(string fileName)
        {
            return Task.Run(delegate
            {
                File.Delete(Path.Combine(appExternalStorage, fileName));
            });
        }

        public Task DeleteDirectory(string pathName)
        {
            return Task.Run(delegate
            {
                Directory.Delete(Path.Combine(appExternalStorage, pathName), true);
            });
        }

        public Task<Stream> OpenFile(string fileName, FileModeEnum fileMode = FileModeEnum.Create)
        {
            return Task.Run<Stream>(delegate
            {
                string fullPath = Path.Combine(appExternalStorage, fileName);
                var parent = Directory.GetParent(fullPath);
                if (!parent.Exists)
                {
                    parent.Create();
                }

                switch (fileMode)
                {
                    case FileModeEnum.Append:
                        return new FileStream(fullPath, FileMode.Append);
                    case FileModeEnum.Create:
                        return new FileStream(fullPath, FileMode.Create);
                    case FileModeEnum.Open:
                        return new FileStream(fullPath, FileMode.Open);
                    default:
                        throw new InvalidProgramException("Unknown fileMode.");
                }

            });
        }

        public Task<Stream> OpenFile1(string fileName, FileModeEnum fileMode = FileModeEnum.Create)
        {
            return Task.Run<Stream>(delegate
            {
                string fullPath = fileName.Replace("file://", "");

                switch (fileMode)
                {
                    case FileModeEnum.Append:
                        return new FileStream(fullPath, FileMode.Append);
                    case FileModeEnum.Create:
                        return new FileStream(fullPath, FileMode.Create);
                    case FileModeEnum.Open:
                        return new FileStream(fullPath, FileMode.Open);
                    default:
                        throw new InvalidProgramException("Unknown fileMode.");
                }

            });
        }
        public Task<bool> DirectoryExists(string filePath)
        {
            return Task.Run<bool>(delegate
            {
                return Directory.Exists(Path.Combine(appExternalStorage, filePath));
            });
        }

        public Task<string[]> GetFiles(string pathName)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

