using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Entity;
using System.Threading;
using System.Threading.Tasks;
namespace LexisNexis.Red.Common.Services
{
    public interface IDeliveryService
    {
        /// <summary>
        /// get dlbook by email and deviceid
        /// </summary>
        /// <returns></returns>
        Task<HttpResponse> ListFileDetails(UserDetails userDetails);

        HttpResponse EndTask(EndTaskRequest request);

        HttpResponse GetCheckContentByTaskID(GetCheckContentRequest request);
        HttpResponse StartTask(StartTaskRequest request);
        /// <summary>
        /// get single dl book information
        /// </summary>
        /// <param name="dlFileDownloadRequest"></param>
        /// <returns></returns>
        Task<HttpResponse> DlFileDetailByDlId(string email, string deviceId, int dlId, CancellationToken token);
        /// <summary>
        /// download dlbook file from remote server
        /// </summary>
        /// <param name="DlBook">DlBook</param>
        /// <param name="cancelToken">cancelToken</param>
        /// <param name="downloadHandler">downloadHandler</param>
        /// <returns>bool</returns>
        Task<bool> DlFileDownload(DlBook dlBook, CancellationToken cancellationToken, DownloadProgressEventHandler downloadHandler);

        /// <summary>
        /// download dictionary
        /// </summary>
        /// <param name="fileUrl"></param>
        /// <param name="dictionaryPath"></param>
        /// <returns></returns>
        Task DictionaryFileDownload(string fileUrl, string dictionaryPath);

        /// <summary>
        /// get latest dictionary version
        /// </summary>
        /// <returns></returns>
        Task<HttpResponse> GetDictionaryVersion();

        /// <summary>
        /// download a small size file from backend
        /// </summary>
        /// <param name="fileUrl">fileUrl</param>
        /// <returns></returns>
        Task<byte[]> DlFileDownload(string fileUrl);

        /// <summary>
        /// get dlversiondfference between lastdownload version  and latest version
        /// </summary>
        /// <param name="dlVersionDifference">dlVersionDifference</param>
        /// <returns></returns>
        Task<HttpResponse> DlFileDifferenceVersion(DlVersionDifference dlVersionDifference);

        /// <summary>
        /// get dlversionchangehistory for special version
        /// </summary>
        /// <param name="DlVersionChangeHistoryRequest">DlVersionChangeHistoryRequest</param>
        /// <returns></returns>
        Task<HttpResponse> DlVersionChangeHistory(DlVersionChangeHistoryRequest dlVersionDifferenceRequest);
        /// <summary>
        /// update remote server dlstatus 
        /// </summary>
        /// <param name="DlFileStatusUpdate">dlFileStatusUpdate</param>
        /// <param name="CancellationToken">token</param>
        /// <returns>bool</returns>
        Task<bool> DlFileStatusUpdate(DlFileStatusUpdate dlFileStatusUpdate, CancellationToken token = default(CancellationToken));
        /// <summary>
        /// get dlbook metadata
        /// </summary>
        /// <param name="dlFileMetadata"></param>
        /// <returns>metadata</returns>
        Task<HttpResponse> DlMetadata(DlMetadata dlFileMetadata);

        Task<HttpResponse> DlFileCurrentVersion(DlFileCurrentVersionRequest dlFileCurrentVersionRequest);

        Task<HttpResponse> DlFileDownloadRequestValidation(string email, string deviceId, int dlId);

    }
}
