namespace LexisNexis.Red.Common.BusinessModel
{
    /// <summary>
    /// download progress integer
    /// </summary>
    /// <param name="downloadedProgress">larger than 0 and less than or equal to 100</param>
    public delegate void DownloadProgressEventHandler(int downloadedProgress,long downloadSize);


}
