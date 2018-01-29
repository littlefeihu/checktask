namespace LexisNexis.Red.Common.BusinessModel
{
    public enum NetworkTypeEnum
    {
        /// <summary>
        /// do not need to care about flow limitation
        /// </summary>
        Normal,
        /// <summary>
        ///  need to care about flow limitation
        /// </summary>
        Mobile,
        /// <summary>
        /// no net work
        /// </summary>
        None
    }
}
