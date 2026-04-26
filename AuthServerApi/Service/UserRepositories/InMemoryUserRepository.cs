using AuthServerApi.Model;
using AuthServerApi.Model.Requests;

namespace AuthServerApi.Service.UserRepositories
{
    public class InMemoryUserRepository : IUserRepository
    {
        private readonly List<User> _users = new List<User>();

        public Task<string> CreateOTP(string Mobile)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<User>> GetAll()
        {
            return Task.FromResult(_users.Take(10));
        }

        public Task<User> GetByEmail(string email)
        {
            return Task.FromResult(_users.FirstOrDefault(u => u.Email == email));
        }

        public Task<User> GetById(Guid userId)
        {
            return Task.FromResult(_users.FirstOrDefault(u => u.Id == userId));
        }

        public Task<User> GetByMobile(string mobile)
        {
            throw new NotImplementedException();
        }

        public Task<User> RegisterUser(User user)
        {
            _users.Add(user);

            return Task.FromResult(user);
        }


        public Task<bool> VerifyOTP(OTPRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
