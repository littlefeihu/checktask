using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.WindowsStore.ViewModels
{
    public enum TOCAccessMode
    {
        NoReload,
        IgnoreHistory,
        All
    }

    public enum RollingDirection
    {
        Up,
        Down
    }
    public enum NavigationType
    {
        TOCDocument,
        History,
        Search,
        Annotation,
        PBO,
        IntraLink,
        InternalLink,
        ExterLink
    }
    public enum ScrollStatue
    {
        up,
        down
    }
    public enum ContextMenuStatue
    {
        LegalDefine,
        Annotation,
        Copy
    }
    public enum TextBlockStatus
    {
        Collapsed,
        Expanded
    }

    public enum NodeExpandStatue
    {
        Leaf,
        Expand,
        Collapse
    }

    public enum AnnotationStatue
    {
        Highlight,
        Note
    }

    public enum TagEditEnum
    {
        New,
        Edit,
        Delete
    }
}
