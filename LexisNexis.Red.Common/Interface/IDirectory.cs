using LexisNexis.Red.Common.BusinessModel;
using System.IO;
using System.Threading.Tasks;
namespace LexisNexis.Red.Common
{
    public interface IDirectory
    {
        /// <summary>
        /// Gets the app root path.
        /// </summary>
        /// <returns>The app root path.</returns>
        string GetAppRootPath();

        /// <summary>
        /// Gets the app external stroage root path.
        /// The external storage root path in specific platform used to storge big files
        /// </summary>
        /// <returns>The app external storage root path.</returns>
        string GetAppExternalRootPath();

        /// <summary>
        /// Creates the directory.
        /// </summary>
        /// <returns><c>true</c>, if directory was created, <c>false</c> otherwise.</returns>
        /// <param name="path">Path.</param>
        Task<bool> CreateDirectory(string path);

        Task<bool> InternalFileExists(string fileName);

        /// <summary>
        /// Determines whether the specified file exist in application internal storage
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        Task<bool> FileExists(string fileName);

        Task<bool> DirectoryExists(string pathName);

        /// <summary>
        /// Save file to application internal storage
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="buffer"></param>
        Task SaveFileToInternal(string fileName, byte[] buffer);

        Task DeleteFile(string fileName);

        /// <summary>
        /// remove all files and folders in this path
        /// </summary>
        /// <param name="pathName"></param>
        Task DeleteDirectory(string pathName);

        /// <summary>
        /// create empty file then return the empty stream for write
        /// </summary>
        /// <param name="fileName"></param>
        Task<Stream> OpenFile(string fileName, FileModeEnum fileMode = FileModeEnum.Create);
        /// <summary>
        ///  Returns the names of files (including their paths) in the specified directory.
        /// </summary>
        /// <param name="pathName"> The directory from which to retrieve the files.</param>
        /// <returns> A String array of file names in the specified directory.</returns>
        Task<string[]> GetFiles(string pathName);

    }

}

