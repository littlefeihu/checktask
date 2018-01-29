using LexisNexis.Red.Common.Entity;
using System.Threading.Tasks;
namespace LexisNexis.Red.Common.Services
{
    public interface IAuthenticationService
    {
        Task<HttpResponse> LoginUserValidation(LoginUser loginUser);
        Task<HttpResponse> ResetPassword(PasswordReset resetUserPassword);
        Task<HttpResponse> PasswordChange(PasswordChange userPasswordChange);
        Task<HttpResponse> RegisterDevice(RegisterDevice registerUserDevice);

    }
}
