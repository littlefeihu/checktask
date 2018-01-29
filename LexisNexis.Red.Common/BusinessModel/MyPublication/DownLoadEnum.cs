namespace LexisNexis.Red.Common.BusinessModel
{
    public enum DownLoadEnum
    {
        Success,
        Failure,
        Canceled,
        /// <summary>
        /// over  flow limitation
        /// </summary>
        OverLimitation,
        NetDisconnected
    }
}
