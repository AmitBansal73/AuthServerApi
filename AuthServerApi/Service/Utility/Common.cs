
namespace AuthServerApi.Service.Utility
{
    public class Common
    {
        private static string salt = "abc";
        public static string PasswordHasher(String password)
        {
            return $"xyz{password}123";
        }
    }
}
