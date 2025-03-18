using AuthServerApi.Data;
using AuthServerApi.Model;
using Microsoft.EntityFrameworkCore;

namespace AuthServerApi.Service.UserRepositories
{
    public class SqlUserRepository : IUserRepository
    {
        private readonly AuthenticationDbContext _context;

        public SqlUserRepository(AuthenticationDbContext authenticationDbContext)
        {
            _context=authenticationDbContext;
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            return await _context.users.Skip(0).Take(10).ToListAsync();
        }

        public async Task<User> GetByEmail(string email)
        {
            return await _context.users.FirstOrDefaultAsync(x => x.Email == email);
        }

         public async Task<User> GetById(Guid userId)
        {
            return await _context.users.FirstOrDefaultAsync(x => x.Id == userId);
        }

        public async Task<User> RegisterUser(User user)
        {
            _context.users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }
    }
}
