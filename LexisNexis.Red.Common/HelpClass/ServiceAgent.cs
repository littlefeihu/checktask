using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Common;
using LexisNexis.Red.Common.DomainService.Events;
using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Common.HelpClass.DomainEvent;
using LexisNexis.Red.Common.HelpClass.Tools;
using LexisNexis.Red.Common.Interface;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
namespace LexisNexis.Red.Common.HelpClass
{
    public class ServiceAgent
    {
        private static readonly HttpClient hclient, pingHclient;
        static ServiceAgent()
        {
            hclient = new HttpClient();
            hclient.DefaultRequestHeaders.Connection.Add("keep-alive");
            hclient.Timeout = new TimeSpan(0, 20, 0);
            pingHclient = new HttpClient();
            pingHclient.Timeout = new TimeSpan(0, 0, 15);
            pingHclient.DefaultRequestHeaders.Add("Accept", Constants.CONTENT_TYPEJSON);
            pingHclient.DefaultRequestHeaders.Connection.Add("keep-alive");
        }

        public static async Task<HttpResponse> RestFullServiceJsonRequest(Uri uri, string uriTemplate, Object serviceRequest, CancellationToken token = default(CancellationToken))
        {
            HttpResponse httpresponse = new HttpResponse { IsSuccess = false };
            JsonSerializerSettings microsoftDateFormatSettings = new JsonSerializerSettings
            {
                DateFormatHandling = DateFormatHandling.MicrosoftDateFormat
            };
            string strEncodeBody = JsonConvert.SerializeObject(serviceRequest, microsoftDateFormatSettings);
            var response = await hclient.PostAsync(new Uri(uri + uriTemplate), new StringContent(strEncodeBody)
            {
                Headers =
                {
                    ContentType = new MediaTypeHeaderValue(Constants.CONTENT_TYPEJSON)
                }
            }, token);
            if (response != null)
            {
                httpresponse.IsSuccess = (response.StatusCode == HttpStatusCode.OK);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    Logger.Log("Response Error" + response.ReasonPhrase);
                    httpresponse.Content = response.ReasonPhrase;
                }
                else
                {
                    httpresponse.Content = await response.Content.ReadAsStringAsync();

                    DomainEvents.Publish(new LastSyncedTimeChangedEvent()).WithNoWarning();
                }

            }
            return httpresponse;
        }

        public static async Task<bool> RestFullPingServiceRequest(Uri uri, string uriTemplate, CancellationToken cancellationToken)
        {
            try
            {
                 var response = await pingHclient.GetAsync(new Uri(uri + uriTemplate), cancellationToken);
                if (response != null)
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        DomainEvents.Publish(new LastSyncedTimeChangedEvent()).WithNoWarning();
                        var status = await response.Content.ReadAsStringAsync();
                        var pingStatus = JsonConvert.DeserializeObject<PingStatus>(status);
                        return pingStatus.Status;
                    }
                }
            }
            catch (OperationCanceledException)
            {//add cancel operation
                if (cancellationToken.IsCancellationRequested)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Unable to reach: " + uri + " Error: " + ex);
            }
            return false;
        }

        public static async Task<bool> RestFullServiceRequestForFileDownload(DlBook dlBook, CancellationToken cancelToken, DownloadProgressEventHandler downloadHandler)
        {
            HttpResponseMessage response = await hclient.GetAsync(new Uri(dlBook.FileUrl), HttpCompletionOption.ResponseHeadersRead, cancelToken);

            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception(Constants.STATUS_CODE_MESSAGE + response.StatusCode);

            return await ResponseStreamToFile(response, cancelToken, dlBook.GetDlBookZipFileName(), dlBook.Size, downloadHandler);
        }
        private static async Task<bool> ResponseStreamToFile(HttpResponseMessage response, CancellationToken cancelToken, string fileName, long size, DownloadProgressEventHandler downloadHandler)
        {
            if (await GlobalAccess.DirectoryService.FileExists(fileName))
            {
                await GlobalAccess.DirectoryService.DeleteFile(fileName);
            }
            using (Stream outputStream = await GlobalAccess.DirectoryService.OpenFile(fileName))
            using (var inputStream = await response.Content.ReadAsStreamAsync())
            {
                byte[] buffer = new byte[Constants.BUFFER_LENGTH];
                int iCount = 0;
                Int64 totalSize = iCount;
                int lastRate = 0;
                do
                {
                    var timeout = new TimeSpan(0, 1, 0);
                    iCount = await inputStream.ReadAsync(buffer, 0, Constants.BUFFER_LENGTH)
                                              .WithCancellation<int>(cancelToken, timeout);
                    if (downloadHandler != null)
                    {
                        int downloadrate = (int)(((double)totalSize / size) * 85);
                        if (downloadrate - lastRate >= 1)
                        {
                            lastRate = downloadrate;
                            if (downloadrate > 85)
                                downloadrate = 85;
                            downloadHandler(downloadrate, totalSize);
                        }
                    }
                    await outputStream.WriteAsync(buffer, 0, iCount).WithCancellation(cancelToken);
                    totalSize += iCount;
                } while (iCount > 0);
            }
            return true;
        }

        public static async Task RestFullServiceRequestForFileDownload(Uri uri, string dictionaryPath)
        {
            HttpResponseMessage response = await hclient.GetAsync(uri);

            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception(Constants.STATUS_CODE_MESSAGE + response.StatusCode);

            await ResponseStreamToFile(response, dictionaryPath);
        }

        private static async Task ResponseStreamToFile(HttpResponseMessage response, string fileName)
        {
            try
            {
                if (await GlobalAccess.DirectoryService.FileExists(fileName))
                {
                    await GlobalAccess.DirectoryService.DeleteFile(fileName);
                }

                var bytesArray = await response.Content.ReadAsByteArrayAsync();

                using (var stream = await GlobalAccess.DirectoryService.OpenFile(fileName, FileModeEnum.Create))
                {
                    await stream.WriteAsync(bytesArray, 0, bytesArray.Length);
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Unable to download dictionary: " + ex.Message);
            }
        }

        public static async Task<byte[]> RestFullServiceRequestForFileDownload(Uri uri)
        {
            HttpResponseMessage response = await hclient.GetAsync(uri);

            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception(Constants.STATUS_CODE_MESSAGE + response.StatusCode);

            return await response.Content.ReadAsByteArrayAsync();
        }
        public static async Task<bool> RestFullServiceRequestForAnnDownwardSync(Uri uri, string uriTemplate, string fileName, Object serviceRequest)
        {
            string strEncodeBody = JsonConvert.SerializeObject(serviceRequest);
            HttpRequestMessage request = new HttpRequestMessage();
            request.Content = new StringContent(strEncodeBody)
            {
                Headers =
                {
                    ContentType = new MediaTypeHeaderValue(Constants.CONTENT_TYPEJSON)
                }
            };
            request.Method = HttpMethod.Post;
            request.RequestUri = new Uri(uri + uriTemplate);
            var response = await hclient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception(Constants.STATUS_CODE_MESSAGE + response.StatusCode);

            return await ResponseStreamToFile(response, default(CancellationToken), fileName, 0, null);
        }

        public static async Task<HttpResponse> RestFullServiceRequestForAnnUpwardSync(Uri uri, string uriTemplate, string strFile)
        {
            HttpResponse httpresponse = new HttpResponse { IsSuccess = false };
            var stream = await GlobalAccess.DirectoryService.OpenFile(strFile, FileModeEnum.Open);
            var response = await hclient.PostAsync(new Uri(uri + uriTemplate), new StreamContent(stream)
            {
                Headers =
                {
                    ContentType = new MediaTypeHeaderValue(Constants.CONTENT_TYPEJSON)
                }
            });
            if (response != null)
            {
                httpresponse.IsSuccess = (response.StatusCode == HttpStatusCode.OK);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    httpresponse.Content = response.ReasonPhrase;
                }
                else
                {
                    httpresponse.Content = await response.Content.ReadAsStringAsync();
                }
            }
            return httpresponse;
        }

    }
}
