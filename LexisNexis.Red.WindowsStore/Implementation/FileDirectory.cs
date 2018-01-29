using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using LexisNexis.Red.Common;
using LexisNexis.Red.Common.BusinessModel;

namespace LexisNexis.Red.WindowsStore.Implementation
{
    public class FileDirectory : IDirectory
    {
        private readonly StorageFolder localFolder = ApplicationData.Current.LocalFolder;

        private string FormatPathName(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                return path.Replace("/", "\\");
            }
            return path;
        }

        public string GetAppRootPath()
        {
            return localFolder.Path;
        }

        public string GetAppExternalRootPath()
        {
            return localFolder.Path;
        }

        public async Task<bool> CreateDirectory(string path)
        {
            try
            {
                path = FormatPathName(path);
                var folder = await localFolder.CreateFolderAsync(path, CreationCollisionOption.OpenIfExists);
                return true;
            }
            catch
            {

                return false;
            }
        }

        public async Task<bool> InternalFileExists(string fileName)
        {
            try
            {
                fileName = FormatPathName(fileName);
                var file = await localFolder.GetFileAsync(fileName);
                return true;
            }
            catch (FileNotFoundException)
            {
                return false;
            }
        }

        public async Task<bool> FileExists(string fileName)
        {
            return await InternalFileExists(fileName);
        }

        public async Task<bool> DirectoryExists(string pathName)
        {
            try
            {
                if (!string.IsNullOrEmpty(pathName))
                {
                    pathName = FormatPathName(pathName);
                    var folder = await localFolder.GetFolderAsync(pathName);

                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task SaveFileToInternal(string fileName, byte[] buffer)
        {
            fileName = FormatPathName(fileName);
            var file = await localFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteBytesAsync(file, buffer);
        }

        public async Task DeleteFile(string fileName)
        {
            fileName = FormatPathName(fileName);
            var file = await localFolder.GetFileAsync(fileName);
            if (file != null)
            {
                await file.DeleteAsync();
            }

        }

        public async Task DeleteDirectory(string pathName)
        {
            pathName = FormatPathName(pathName);
            var folder = await localFolder.GetFolderAsync(pathName);
            if (folder != null)
            {
                await folder.DeleteAsync();
            }
        }

        public async Task<Stream> OpenFile(string fileName, FileModeEnum fileMode = FileModeEnum.Create)
        {
            fileName = FormatPathName(fileName);
            Stream stream = null;
            switch (fileMode)
            {
                case FileModeEnum.Create:
                    stream = await localFolder.OpenStreamForWriteAsync(fileName, CreationCollisionOption.ReplaceExisting);
                    break;
                case FileModeEnum.Open:
                    stream = await localFolder.OpenStreamForReadAsync(fileName);
                    break;
                case FileModeEnum.Append:
                    stream = await localFolder.OpenStreamForWriteAsync(fileName, CreationCollisionOption.OpenIfExists);
                    stream.Seek(0, SeekOrigin.End);
                    break;
            }
            return stream;
        }

        public async Task<string[]> GetFiles(string pathName)
        {
            pathName = FormatPathName(pathName);
            var folder = await localFolder.GetFolderAsync(pathName);
            var files = await folder.GetFilesAsync();
            string[] filesName = new string[files.Count];
            for (int i = 0; i < filesName.Length; i++)
            {
                filesName[i] = files[i].Path;
            }
            return filesName;
        }
    }
}
