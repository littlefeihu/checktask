using LexisNexis.Red.WindowsStore.ViewModels;
using Microsoft.Practices.Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.WindowsStore.Html
{
    public class NavigationRecord
    {
        public int BookId { get; set; }
        public int TOCId { get; set; }
        public int PageNum { get; set; }
        public NavigationType Type { get; set; }
        public object Tag { get; set; }
    }
    public class NavigationRecordManager : BaseViewModel
    {
        public NavigationRecordManager()
        {
            HistoryRecords = new List<NavigationRecord>();
            CurrenRecordIndex = -1;
        }

        public List<NavigationRecord> HistoryRecords { get; set; }
        public int CurrenRecordIndex { get; set; }
        private bool backWard = false;
        public bool BackWard
        {
            get { return backWard; }
            set { SetProperty(ref backWard, value); }
        }

        private bool forWard = false;
        public bool ForWard
        {
            get { return forWard; }
            set { SetProperty(ref forWard, value); }
        }

        public void Record(int bookId, int tocID, NavigationType type = NavigationType.TOCDocument, int pageNum = 0, object tag = null)
        {
            NavigationRecord record = new NavigationRecord
            {
                BookId = bookId,
                TOCId = tocID,
                PageNum = pageNum,
                Type = type,
                Tag = tag
            };
            if (CurrenRecordIndex == HistoryRecords.Count - 1)
            {
                CurrenRecordIndex++;
                HistoryRecords.Add(record);
            }
            else if (CurrenRecordIndex < HistoryRecords.Count - 1)
            {
                CurrenRecordIndex++;
                HistoryRecords.RemoveRange(CurrenRecordIndex, HistoryRecords.Count - CurrenRecordIndex);
                HistoryRecords.Add(record);
            }
            BackWard = CurrenRecordIndex > 0 ? true : false;
            ForWard = false;
        }

        public NavigationRecord GoForward()
        {
            CurrenRecordIndex++;
            ForWard = CurrenRecordIndex < HistoryRecords.Count - 1 ? true : false;
            BackWard = true;
            return HistoryRecords[CurrenRecordIndex];
        }
        public NavigationRecord GoBack()
        {
            CurrenRecordIndex--;
            ForWard = true;
            BackWard = CurrenRecordIndex > 0 ? true : false;
            return HistoryRecords[CurrenRecordIndex];
        }

        public NavigationRecord GetCurrentRecord()
        {
            return HistoryRecords[CurrenRecordIndex];
        }
    }
}
