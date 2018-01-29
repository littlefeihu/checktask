using LexisNexis.Red.Common.Common;
using LexisNexis.Red.Common.HelpClass;
using SQLite;
using System;
using System.Collections.Generic;

namespace LexisNexis.Red.Common.BusinessModel
{

    public sealed class Publication
    {
        private DateTime? validTo;
        public int RowId { get; set; }
        public int BookId
        {
            get;
            set;
        }
        public string Name
        {
            get;
            set;
        }

        public string CountryCode
        {
            get;
            set;
        }
        public string ShelfViewBookTitle
        {
            get
            {
                if (!string.IsNullOrEmpty(this.Name))
                {
                    if (this.Name.Trim().Length > 45)
                        return this.Name.Trim().Substring(0, 45) + "...";
                    else
                        return this.Name;
                }
                else
                {
                    return "";
                }
            }
        }
        public string Author
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }
        /// <summary>
        /// lasted version on server
        /// </summary>
        public int CurrentVersion
        {
            get;
            set;
        }
        public long Size
        {
            get;
            set;
        }

        public DateTime? LastUpdatedDate
        {
            get;
            set;
        }
        /// <summary>
        /// last download version  on local 
        /// </summary>
        public int LastDownloadedVersion
        {
            get;
            set;
        }

        public string DpsiCode
        {
            get;
            set;
        }
        /// <summary>
        /// judge the book as   Full Text Case  by the dpsicode
        /// </summary>
        public bool IsFTC
        {
            get
            {
                //return this.DpsiCode.StartsWith("P-");
                return false;
            }
        }
        public string FileUrl
        {
            get;
            set;
        }
        public string InitVector
        {
            get;
            set;
        }


        public string K2Key
        {
            get;
            set;
        }


        public string HmacKey
        {
            get;
            set;
        }


        public string ColorPrimary
        {
            get;
            set;
        }
        public string ColorSecondary
        {
            get;
            set;
        }

        public string FontColor
        {
            get;
            set;
        }


        public bool IsLoan
        {
            get;
            set;
        }

        public bool IsTrial
        {
            get;
            set;
        }


        private DateTime? originValidTo;
        public DateTime? ValidTo
        {
            get
            {
                if ((this.IsTrial || this.IsLoan))
                {
                    var daysRemaining = this.originValidTo.Value.DaysRemaining();
                    if (daysRemaining < 0)
                    {//set trial or loan dlbook to due to expired
                        this.validTo = DateTime.Now.Date;
                    }
                    else
                    {//set validdate to origin valid value
                        this.validTo = this.originValidTo;
                    }
                }
                return this.validTo;
            }
            set
            {
                this.originValidTo = value;
                this.validTo = value;
            }
        }

        /// <summary>
        /// indicate the loaded book expire date remaining
        /// </summary>
        public int DaysRemaining
        {
            get
            {
                if (this.validTo != null)
                {
                    return this.ValidTo.Value.DaysRemaining();
                }
                else
                {
                    return -1;
                }
            }
        }

        public PublicationStatusEnum PublicationStatus
        {
            get;
            set;
        }

        /// <summary>
        /// differ between lastedversion and localversion
        /// </summary>
        public int UpdateCount { get; set; }


        public string PracticeArea { get; set; }

        public string SubCategory { get; set; }

        public DateTime? InstalledDate { get; set; }

        public long LocalSize { get; set; }

        public DateTime? CurrencyDate { get; set; }

        public int OrderBy { get; set; }


        /// <summary>
        /// Deleted Guide Card List
        /// </summary>
        public List<GuideCard> DeletedGuideCard { get; set; }

        /// <summary>
        /// Newly Included Guide Card
        /// </summary>
        public List<GuideCard> AddedGuideCard { get; set; }

        /// <summary>
        /// Updated Guide Card
        /// </summary>
        public List<GuideCard> UpdatedGuideCard { get; set; }


    }
}
