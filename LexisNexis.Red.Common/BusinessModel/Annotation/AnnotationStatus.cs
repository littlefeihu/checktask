using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.BusinessModel
{
    public enum AnnotationStatusEnum
    {
        /// <summary>
        /// new created and unsynced
        /// </summary>
        Created = 1,
        /// <summary>
        /// new updated and unsynced
        /// </summary>
        Updated = 2,
        /// <summary>
        /// deleted and unsynced
        /// </summary>
        Deleted = 3,
        /// <summary>
        /// Orphaned ,which has no response file 
        /// </summary>
        Orphaned = 4
    }
}
