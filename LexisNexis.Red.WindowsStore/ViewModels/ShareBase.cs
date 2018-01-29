using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;

namespace LexisNexis.Red.WindowsStore.ViewModels
{
    public class ShareBase
    {
        public DataTransferManager DTTransferManager { set; get; }
        public ShareBase()
        {
            DTTransferManager = DataTransferManager.GetForCurrentView();
        }

        public void ShowShareUI()
        {
            DataTransferManager.ShowShareUI();
        }
    }
}
