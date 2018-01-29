using LexisNexis.Red.Common.BusinessModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.HelpClass
{
    public class TagEqualityComparer : IEqualityComparer<AnnotationTag>
    {
        public bool Equals(AnnotationTag tag1, AnnotationTag tag2)
        {
            return (tag1.TagId == tag2.TagId);
        }
        public int GetHashCode(AnnotationTag annotationTag)
        {
            return annotationTag.TagId.GetHashCode();
        }

    }
}
