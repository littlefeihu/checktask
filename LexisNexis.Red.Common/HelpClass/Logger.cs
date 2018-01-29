using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Common;
using LexisNexis.Red.Common.Database;
using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Common.Interface;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace LexisNexis.Red.Common.HelpClass
{
    public static class Logger
    {
        private static RedLock redLock = new RedLock(1);
        public static void Log(string message)
        {
            try
            {
                LogProcess(message).Wait();
            }
            catch (Exception)
            {

            }
        }

        private static Task LogProcess(string message)
        {
            return Task.Run(async () =>
            {
                try
                {
                    await redLock.Enter();
                    if (!await GlobalAccess.DirectoryService.DirectoryExists(Constants.LOGGER_NAME))
                    {
                        await GlobalAccess.DirectoryService.CreateDirectory(Constants.LOGGER_NAME);
                    }
                    var loggerFileName = Path.Combine(Constants.LOGGER_NAME, DateTime.Now.ToString("yyyyMMdd", CultureInfo.CurrentCulture) + Constants.TXT_SUFFIXNAME);
                    if (!await GlobalAccess.DirectoryService.FileExists(loggerFileName))
                    {
                        await Write(loggerFileName, message, FileModeEnum.Create);
                        //remove yesterday's log
                        string oldLoggerFileName = Path.Combine(Constants.LOGGER_NAME, DateTime.Now.AddDays(-1).ToString("yyyyMMdd", CultureInfo.CurrentCulture) + Constants.TXT_SUFFIXNAME);
                        if (await GlobalAccess.DirectoryService.FileExists(oldLoggerFileName))
                        {
                            await GlobalAccess.DirectoryService.DeleteFile(oldLoggerFileName);
                        }
                    }
                    else
                    {
                        await Write(loggerFileName, message, FileModeEnum.Append);
                    }
                }
                finally
                {
                    redLock.Release();
                }
            });

        }
        private static async Task Write(string fileName, string message, FileModeEnum fileMode)
        {
            using (var stream = await GlobalAccess.DirectoryService.OpenFile(fileName, fileMode))
            {
                var bytes = Encoding.UTF8.GetBytes("\r\n" + DateTime.Now.ToLocalTime() + ",info:" + message);
                await stream.WriteAsync(bytes, 0, bytes.Length);
            }
        }
    }
}
