using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.DominService
{
    public interface IPublicationDomainService
    {
        IEnumerable<DlBook> GetAdditionalDlBooks(IEnumerable<DlBook> exceptedDlBooks);
        Task DeleteDlBooksByEreader(IEnumerable<DlBook> toBeDeletedDlBooks);
        void VerifyNetwork(long localSize);
        Task DeleteDlbookByUser(int bookId, UserCredential userCredential);
        Task<DlVersionChangeHistoryResponse> GetDlBookChangeHistory(int bookId, int lastDownloadedVersion, string email, string deviceId);
        Task RemoveDlBooksFromServer(UserCredential userCredential);
        Task<DlBook> RefreshLocalDlBook(DlBook serverDlBook, DlBook localDlBook, string deviceid);
        IEnumerable<Publication> UpdateLocalDlBooks(List<DlBook> onlineDlBooks, List<DlBook> localDlBooks);
        Task<DlBook> GetLatestDlBookDetail(UserCredential userCredential, string deviceId, int bookId, CancellationToken cancelToken);
        Task DownLoadDlBook(DlBook serverDlBook, CancellationToken cancelToken, DownloadProgressEventHandler downloadHandler);
        Task InstallDlBook(DlBook dlBook, string symmetricKey, CancellationToken cancelToken);
        Task<List<DlBook>> GetOnlineDlBooks(string email);
        Task ResetDlBookOrder(List<int> bookIds, UserCredential userCredential);
    }
}
