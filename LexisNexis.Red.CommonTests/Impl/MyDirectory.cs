using LexisNexis.Red.Common;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.HelpClass;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.CommonTests.Impl
{
    public class MyDirectory : IDirectory
    {
        public string GetAppRootPath()
        {
            return System.Configuration.ConfigurationManager.AppSettings["AppRootPath"];
        }

        public string GetAppExternalRootPath()
        {
            return GetAppRootPath();
        }

        public async Task<bool> CreateDirectory(string path)
        {
            try
            {
                return await Task.Run(() =>
                {
                    path = Path.Combine(GetAppRootPath(), path);
                    Directory.CreateDirectory(path);
                    return true;
                });
            }
            catch
            {

            }
            return await Task.Run(() => { return false; });
        }

        public async Task<bool> FileExists(string fileName)
        {
            return await Task.Run(() =>
            {
                fileName = Path.Combine(GetAppRootPath(), fileName);
                return File.Exists(fileName);
            });
        }

        public async Task<bool> DirectoryExists(string filePath)
        {
            return await Task.Run(() =>
            {
                filePath = Path.Combine(GetAppRootPath(), filePath);
                return Directory.Exists(filePath);
            });
        }

        public async Task SaveFileToInternal(string fileName, byte[] buffer)
        {
            await Task.Run(() =>
            {
                File.WriteAllBytes(Path.Combine(GetAppRootPath(), fileName), buffer);
            });
        }

        public async Task DeleteFile(string fileName)
        {
            await Task.Run(() =>
            {
                fileName = Path.Combine(GetAppRootPath(), fileName);
                File.Delete(fileName);
            });
        }


        public async Task DeleteDirectory(string pathName)
        {
            await Task.Run(() =>
            {
                pathName = Path.Combine(GetAppRootPath(), pathName);
                Directory.Delete(pathName, true);
            });
        }

        public async Task<bool> InternalFileExists(string fileName)
        {
            return await Task.Run(() =>
            {

                return File.Exists(fileName = Path.Combine(GetAppRootPath(), fileName));
            });
        }


        public async Task<Stream> OpenFile(string fileName, Common.BusinessModel.FileModeEnum fileMode = FileModeEnum.Create)
        {
            string fullFileName = Path.Combine(GetAppRootPath(), fileName);
            return await Task.Run<Stream>(() =>
            {
                switch (fileMode)
                {
                    case FileModeEnum.Create:
                        return File.Open(fullFileName, FileMode.Create);
                    case FileModeEnum.Open:
                        return File.Open(fullFileName, FileMode.Open);
                    default:
                        return File.Open(fullFileName, FileMode.Append);
                }
            });
        }


        public async Task<string[]> GetFiles(string pathName)
        {
            return await Task.Run(() =>
            {

                return Directory.GetFiles(Path.Combine(GetAppRootPath(), pathName));

            });
        }
    }

}
