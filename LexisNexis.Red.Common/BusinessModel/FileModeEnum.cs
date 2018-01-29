using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.BusinessModel
{
    // Summary:
    //     Specifies how the operating system should open a file.
    public enum FileModeEnum
    {
        //
        // Summary:
        //     Specifies that the operating system should create a new file. If the file
        //     already exists, it will be overwritten. This operation requires System.Security.Permissions.FileIOPermissionAccess.Write
        //     permission. System.IO.FileMode.Create is equivalent to requesting that if
        //     the file does not exist, use System.IO.FileMode.CreateNew; otherwise, use
        //     System.IO.FileMode.Truncate. If the file already exists but is a hidden file,
        //     an System.UnauthorizedAccessException exception is thrown.
        Create,
        //
        // Summary:
        //     Specifies that the operating system should open an existing file. The ability
        //     to open the file is dependent on the value specified by the System.IO.FileAccess
        //     enumeration. A System.IO.FileNotFoundException exception is thrown if the
        //     file does not exist.
        Open,
        //
        // Summary:
        //     Opens the file if it exists and seeks to the end of the file, or creates
        //     a new file. This operation requires System.Security.Permissions.FileIOPermissionAccess.Append
        //     permission. FileMode.Append can be used only in conjunction with FileAccess.Write.
        //     Trying to seek to a position before the end of the file throws an System.IO.IOException
        //     exception, and any attempt to read fails and throws an System.NotSupportedException
        //     exception.
        Append,
    }
}
