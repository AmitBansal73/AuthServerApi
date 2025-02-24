using AuthServerApi.Model;

namespace AuthServerApi.Service.UserRepositories
{
    public interface IUserRepository
    {
        Task<User> GetByEmail(string email);

        Task<User> GetById(Guid userId);

        Task<User> RegisterUser(User user);
    }
}
