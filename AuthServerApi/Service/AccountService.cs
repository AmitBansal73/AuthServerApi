using AuthServerApi.Exceptions;
using AuthServerApi.Model;
using AuthServerApi.Service.UserRepositories;
using System.Security.Claims;

namespace AuthServerApi.Service
{
    public class AccountService : IAccountService
    {
        IUserRepository _userRepository;
        public AccountService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<User> LoginWithGoogleAsync(ClaimsPrincipal claimsPrincipal)
        {
            if (claimsPrincipal == null) { 
            throw new ExternalLoginProviderException("Google","Claims Principal is null");
            }
            var email = claimsPrincipal.FindFirstValue(ClaimTypes.Email);
            
            if (email == null) {
                throw new ExternalLoginProviderException("Google", "Email is null");
            }

            var user = await _userRepository.GetByEmail(email);
            //var dob = claimsPrincipal.FindFirstValue(ClaimTypes.DateOfBirth) ?? DateTime.MinValue.ToString();
            if (user == null) {
                var newUser = new User
                {
                    Name = claimsPrincipal.FindFirstValue(ClaimTypes.Name) ?? string.Empty,
                    Email = email,
                    Id = Guid.NewGuid(),
                    MobileNumber = claimsPrincipal.FindFirstValue(ClaimTypes.MobilePhone) ?? string.Empty,
                    HashPassword = string.Empty
                };
                var result = await _userRepository.RegisterUser(newUser);
                if (result == null)
                {
                    throw new ExternalLoginProviderException("Google", "Unable to create user");
                }

                user = newUser;
            }

            return user;

        }
    }
}
