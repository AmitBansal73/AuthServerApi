using AuthServerApi.Model;
using Microsoft.EntityFrameworkCore;

namespace AuthServerApi.Data
{
    public class AuthenticationDbContext : DbContext
    {
        public AuthenticationDbContext(DbContextOptions<AuthenticationDbContext> option)
            : base(option) { 
        
        }

        public DbSet<User> users { get; set; }
        public DbSet<RefreshToken> refreshTokens { get; set; }
    }
}
