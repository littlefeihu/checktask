namespace LexisNexis.Red.Common.BusinessModel
{
    public enum PasswordChangeEnum
    {
        /// <summary>
        ///eagle70: 4. Error message for non-matching password criteria 
        /// </summary>
        LengthInvalid = 1,
        /// <summary>
        ///eagle70:  5. Error message for mismatched new password fields 
        /// </summary>
        NotMatch,
        ChangeFailure,
        NetDisconnected,
        ChangeSuccess
    }
}
