using LexisNexis.Red.Common;
using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Common;
using LexisNexis.Red.Common.HelpClass;
using LexisNexis.Red.Common.Interface;
using LexisNexis.Red.CommonTests.Impl;
using LexisNexis.Red.CommonTests.Interface;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LexisNexis.Red.CommonTests.Business.PingTest
{
    public class PingTest
    {
        IConnectionMonitor connectionMonitor = new ConnectionMonitor();
        HttpClient _httpClient = new HttpClient();
        public PingTest()
        {
            _httpClient.Timeout = new TimeSpan(0, 0, 10);
            _httpClient.DefaultRequestHeaders.Add("Accept", Constants.CONTENT_TYPEJSON);
        }
        [Test]
        public async void Log()
        {
            try
            {

                 Logger.Log("-------------------------------------");
                Stopwatch timer = new Stopwatch();

                for (int i = 0; i < 20; i++)
                {
                    timer.Start();
                    await connectionMonitor.PingService("AU");
                    await ServiceAgent.RestFullPingServiceRequest(new Uri("http://192.168.1.101/ln.red/service/ds.webservice/AuthenticationService.svc"), "/Islive", new CancellationToken());
                    timer.Stop();
                     Logger.Log(timer.ElapsedMilliseconds.ToString());
                    timer.Reset();
                }

            }
            catch (Exception)
            {
                throw;
            }

        }
        [Test]
        public async void CancelPingTask()
        {
            try
            {
                for (int i = 0; i < 10; i++)
                {
                    try
                    {
                        CancellationTokenSource source = new CancellationTokenSource();
                        source.CancelAfter(3000);
                        var pingResult = await connectionMonitor.PingService("AU", source.Token);

                    }
                    catch (OperationCanceledException)
                    {
                        Logger.Log("OperationCanceled");
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        [Test,Ignore]
        public async void CancelDownloadTask()
        {
            await LoginUtil.Instance.ValidateUserLogin("allen@lexisred.com", "1234", "AU");
            DownloadResult downloadResult = new DownloadResult();
            int bookId = 35;
            try
            {
                //for (int i = 0; i < 3; i++)
                //{
                CancellationTokenSource source = new CancellationTokenSource();
                source.CancelAfter(2000);
                //download this book 
                downloadResult = await PublicationUtil.Instance.DownloadPublicationByBookId(bookId, source.Token, new DownloadProgressEventHandler((bytes, downloadSize) =>
               {
                   Debug.Write(bytes);
               }));
                Assert.IsTrue(downloadResult.DownloadStatus == DownLoadEnum.Canceled);
                //}
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<string> GetAsync()
        {
            var response = await _httpClient.GetAsync("http://192.168.1.101/ln.red/service/ds.webservice/AuthenticationService.svc/Islive");

            return await response.Content.ReadAsStringAsync();
        }

    }
}
