using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Common;
using LexisNexis.Red.Common.Database;
using LexisNexis.Red.Common.HelpClass;
using LexisNexis.Red.Common.HelpClass.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.Entity
{
    public class PublicationContent
    {
        byte[] contentKey;
        DlBook dlBook;
        public PublicationContent(DlBook dlBook, byte[] contentKey)
        {
            this.contentKey = contentKey;
            this.dlBook = dlBook;
        }
        internal DlBook DlBook
        {
            get
            {
                return dlBook;
            }
        }
        public string DecryptedDbFullName
        {
            get
            {
                return dlBook.GetDecryptedDbFullName();
            }
        }
        public string XpathDbFullName
        {
            get
            {
                return dlBook.GetXpathDbFullName();
            }
        }

        public bool HasPage { get; internal set; }
        public List<PageItem> Pages { get; internal set; }
        public async Task DecryptDataBase()
        {
            await dlBook.DecryptDataBase(contentKey);
        }
        public string UpdateContentImgSrcValue(string content)
        {
            Regex regex = new Regex(Constants.CONTENT_IMAGES_PLACEHOLDER, RegexOptions.IgnoreCase);
            string imgSrc = Path.Combine(GlobalAccess.DirectoryService.GetAppExternalRootPath(),
                                        DlBook.GetDirectory() + "\\");
            content = regex.Replace(content, imgSrc);
            return content;
        }
        public async Task<string> DecryptIndexContent(string fileName)
        {
            return await DecryptContent(fileName, true);
        }
        public async Task<string> DecryptTOCContent(string fileName)
        {
            return await DecryptContent(fileName, false);
        }
        private async Task<string> DecryptContent(String fileName, bool IsIndexFile = false)
        {
            string contentString = string.Empty;
            string fullName = null;
            if (IsIndexFile)
            {
                fullName = Path.Combine(dlBook.GetDirectory(), "index", fileName.Replace(Constants.XML, Constants.ZIP));
            }
            else
            {
                fullName = Path.Combine(dlBook.GetDirectory(), fileName.Replace(Constants.XML, Constants.ZIP));
            }
            if (await GlobalAccess.DirectoryService.FileExists(fullName))
            {
                contentString += await DecryptManager.DecryptContentFromZipFile(fullName, this.contentKey, dlBook.GetInitVector());
            }
            else
            {
                Logger.Log("Missing file");
            }


            return contentString;
        }

    }
}
