using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;

namespace LexisNexis.Red.WindowsStore.Services
{
    public class LogService
    {
        public static void RecordUnhandledException(UnhandledExceptionEventArgs e)
        {
            StringBuilder log = new StringBuilder();           
            log.AppendLine("Date: " + DateTime.Now.ToString("hh:mm tt, dd MMM yyyy"));
            log.AppendLine("Message: " + e.Message);
            log.AppendLine("Source: " + e.Exception.Source);
            log.AppendLine("StackTrace: " + e.Exception.StackTrace);
            log.AppendLine("InnerException: " + e.Exception.Message);
            log.Append("Exception: " + e.ToString()+"\n");
            if(e.Exception.InnerException!=null)
            {
                log.AppendLine("InnerMessage: " + e.Exception.InnerException.Message);
            }
            log.AppendLine("-------------------------------------------------------\n");

            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            Task.Run(async () =>
                {
                    var logFolder = await localFolder.CreateFolderAsync("RedLogger", CreationCollisionOption.OpenIfExists);
                    var logFile = await logFolder.CreateFileAsync("UnhandledException.txt", CreationCollisionOption.OpenIfExists);
                    var logStream = await logFile.OpenAsync(FileAccessMode.ReadWrite);
                    logStream.Seek(logStream.Size);
                    using (StreamWriter sw = new StreamWriter(logStream.AsStreamForWrite(), Encoding.UTF8))
                    {
                        sw.Write(log.ToString());
                    }
                }).Wait();
           
        }
    }
}
