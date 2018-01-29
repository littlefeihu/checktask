using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.Entity
{
    public class Annotations
    {
        [PrimaryKey, AutoIncrement]
        public int RowId { get; set; }
        public string Email { get; set; }
        public string ServiceCode { get; set; }
        public Guid AnnotationCode { get; set; }
        public int BookID { get; set; }
        public string DocumentID { get; set; }
        public int AnnotationType { get; set; }
        public int Status { get; set; }
        public bool IsSynced { get; set; }
        public string AnnotationContent { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public DateTime LastSyncDate { get; set; }
        public string NoteText { get; set; }
        public string HighlightText { get; set; }
    }
}
