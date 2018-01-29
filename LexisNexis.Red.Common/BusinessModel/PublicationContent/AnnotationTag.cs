using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.BusinessModel
{
    public class AnnotationTag
    {
        public Guid TagId { get; set; }
        public string Color { get; set; }
        public string Title { get; set; }
        public int SortOrder { get; set; }
        public override string ToString()
        {
            return TagId.ToString() + (!string.IsNullOrEmpty(Color) ? Color.ToString() : string.Empty) + (!string.IsNullOrEmpty(Title) ? Title.ToString() : string.Empty);
        }
    }

}
