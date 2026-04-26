using AuthServerApi.Data;
using AuthServerApi.Model;
using AuthServerApi.Model.Requests;
using Microsoft.EntityFrameworkCore;

namespace AuthServerApi.Service.UserRepositories
{
    public class SqlUserRepository : IUserRepository
    {
        private readonly AuthenticationDbContext _context;

        public SqlUserRepository(AuthenticationDbContext authenticationDbContext)
        {
            _context = authenticationDbContext;
        }

        public async Task<string> CreateOTP(string Mobile)
        {
            var entry = await _context.MobileOTPs.FirstOrDefaultAsync(
                        x => x.MobileNumber == Mobile);

            if (entry != null)
            {
                _context.Remove(entry);
                _context.SaveChanges();
            }
            //var otpCode = RandomNumberGenerator.GetInt32(1000, 10000).ToString();
            var otpCode = Mobile.Substring(Mobile.Length - 4);

            var otp = new MobileOTP { MobileNumber = Mobile, OTP = otpCode, CreatedAt = DateTime.UtcNow };
            _context.MobileOTPs.Add(otp);
            await _context.SaveChangesAsync();
            return otpCode;
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

        public async Task<User> GetByMobile(string mobile)
        {
            return await _context.users.FirstOrDefaultAsync(x => x.MobileNumber == mobile);
        }


        public async Task<User> RegisterUser(User user)
        {
            _context.users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }


        public async Task<bool> VerifyOTP(OTPRequest request)
        {
            var entry = await _context.MobileOTPs.FirstOrDefaultAsync(
                x => x.MobileNumber == request.MobileNumber
                && x.OTP == request.OTP && DateTime.UtcNow <= x.CreatedAt.AddSeconds(120));

            if (entry == null) return false;

            _context.MobileOTPs.Remove(entry);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
