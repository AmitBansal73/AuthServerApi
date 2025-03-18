using AuthServerApi.Model;
using System.Security.Claims;

namespace AuthServerApi.Service
{
    public interface IAccountService
    {
        Task<User> LoginWithGoogleAsync(ClaimsPrincipal principal);
    }
}
