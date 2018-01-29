using LexisNexis.Red.Common.HelpClass;
using LexisNexis.Red.Common.Interface;
using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
namespace LexisNexis.Red.Common.Entity
{
    public partial class DlBook
    {

     
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

        [JsonProperty("IsTrial")]
        public bool IsTrial
        {
            get;
            set;
        }

        [JsonProperty("ValidTo")]
        public DateTime? ValidTo
        {
            get;
            set;
        }

        public string Email { get; set; }

        public string ServiceCode { get; set; }

        public string CountryCode { get; set; }

        [PrimaryKey, AutoIncrement]
        public int RowId { get; set; }

        public short DlStatus { get; set; }

        public int OrderBy { get; set; }

        public string PracticeArea { get; set; }

        public string SubCategory { get; set; }

        public DateTime? InstalledDate { get; set; }

        public long LocalSize { get; set; }

        public DateTime? CurrencyDate { get; set; }

        public string AddedGuideCard { get; set; }
        public string DeletedGuideCard { get; set; }
        public string UpdatedGuideCard { get; set; }

    }

}
