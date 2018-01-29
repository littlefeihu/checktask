using Newtonsoft.Json;
namespace LexisNexis.Red.Common.Entity
{
    public class LoginUserValidationResponse
    {
        /// <summary>
        /// Used to indicate whether login user is valid user or not.
        /// Possible values:
        /// "true" if login user is valid.
        /// "false" if login user is invalid.
        /// </summary>
        public bool ValidUser { get; set; }

        /// <summary>
        /// User to store first name of the login user.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// User to store last name of the login user.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Used to indicate whether password given by user is machine generator or not.
        /// Possible Values:
        /// "true" if login user password is machine generated.
        /// "false" if login user password is not machine generated.
        /// </summary>
        public bool MachineGeneratedPassword { get; set; }

        /// <summary>
        /// Used to indicate whether user account is currently locked or not.
        /// Possible Values:
        /// "true" if login user account is locked.
        /// "false" if login user account is not locked.
        /// </summary>
        public bool Locked { get; set; }

        /// <summary>
        /// Used to indicate whether user is first time login.
        /// Possible Values:
        /// "true" if login user account is first time login.
        /// "false" if login user account is not first time login.
        /// </summary>
        public bool FirstLogin { get; set; }

        /// <summary>
        /// Used to indicate whether to sync annotations to server.
        /// Possible Values:
        /// "true" if app can sync and save annotations to server.
        /// "false" if app is not allowed to sync and save annotations to server.
        /// </summary>
        public bool SaveAnnotationOnSync { get; set; }

        [JsonProperty("emailaccountisexistsfield")]
        public bool EmailAccountIsExists { get; set; }

        public string state { get; set; }


    }
}

