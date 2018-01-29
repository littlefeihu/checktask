namespace LexisNexis.Red.Common.BusinessModel
{
    public enum LoginStatusEnum
    {
        /// <summary>
        ///eagle70: 2.Display message for when connectivity unavailable 
        /// </summary>
        NetDisconnected,
        /// <summary>
        /// eagle70: 7. Error message from invalid email (login) 
        /// </summary>
        InvalidemailAndValidPwd,
        /// <summary>
        ///eagle70:  8. Error message from invalid password (login) 
        /// </summary>
        ValidemailAndInvalidPwd,
        /// <summary>
        ///eagle70:  9. Error message from invalid email and invalid password (login) 
        /// </summary>
        InvalidemailAndInvalidPwd,
        /// <summary>
        ///eagle70:  10. Error message from valid email and empty password (login) 
        /// </summary>
        ValidemailAndEmptyPwd,
        /// <summary>
        ///eagle70:  11. Error message from empty email and valid password (login) 
        /// </summary>
        EmptyemailAndValidPwd,
        /// <summary>
        /// empty email and invalid pwd
        /// </summary>
        EmptyemailAndInvalidPwd,
        /// <summary>
        ///eagle70:  12. Error message from invalid email and empty password (login) 
        /// </summary>
        InvalidemailAndEmptyPwd,


        /// <summary>
        /// email or password error
        /// </summary>
        EmailOrPwdError,
        /// <summary>
        /// empty email and empty password
        /// </summary>
        EmptyEmailAndEmptyPwd,





        //EmptyEmial,
        //EmptyPassword,
        SelectCountry,
        /// <summary>
        /// DeviceLimit
        /// </summary>
        DeviceLimit,
        /// <summary>
        /// Login success
        /// </summary>
        LoginSuccess,
        /// <summary>
        ///fail to login
        /// </summary>
        LoginFailure,
        /// <summary>
        /// the user account is locked by administrator
        /// </summary>
        AccountLocked,
        InvalidEmail,
        InvalidPassword,
        /// <summary>
        ///eagle70:  19. Invalid email address (snapshot attached) 
        /// </summary>
        AccountNotExist

    }
}