using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.BusinessModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.WindowsStore.Tools
{
    public static class AnnotationTools
    {
        public static readonly List<TagColor> TagColors = AnnCategoryTagUtil.Instance.GetTagColors();
    }
}
