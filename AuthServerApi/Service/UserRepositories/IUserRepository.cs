using AuthServerApi.Model;
using AuthServerApi.Model.Requests;

namespace AuthServerApi.Service.UserRepositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAll();
        Task<User> GetByEmail(string email);

        Task<User> GetByMobile(string mobile);

        Task<User> GetById(Guid userId);

        Task<User> RegisterUser(User user);

        Task<String> CreateOTP(String Mobile);

        Task<bool> VerifyOTP(OTPRequest request);

    }
}
