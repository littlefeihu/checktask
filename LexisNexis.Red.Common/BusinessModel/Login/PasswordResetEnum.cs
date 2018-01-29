namespace LexisNexis.Red.Common.BusinessModel
{
    public enum PasswordResetEnum
    {
        /// <summary>
        /// EmailNotExist
        /// </summary>
        EmailNotExist,
        /// <summary>
        /// ResetSuccess
        /// </summary>
        ResetSuccess,
        /// <summary>
        ///eagle130: 2. Error message from invalid email (Reset password screen) 
        /// </summary>
        InvalidEmail,
        /// <summary>
        /// eagle130:4. Device id not mapped to email address (snapshot attached) 
        /// </summary>
        DeviceIdNotMatched,
        /// <summary>
        /// need to select a country
        /// </summary>
        SelectCountry,
        /// <summary>
        /// NetDisconnected
        /// </summary>
        NetDisconnected,
        /// <summary>
        /// ResetFailure
        /// </summary>
        ResetFailure
    }
}
