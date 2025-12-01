using Microsoft.AspNetCore.Identity;

namespace Simple_Bank_Teller.Helper
{
    public class PasswordHash
    {
        private static readonly PasswordHasher<object> hasher_ = new();

        public static string HashPassword(string password)
        {
            return hasher_.HashPassword(null, password);
        }

        public static bool VerifyPassword(string hashpassword, string providedPassword)
        {
            var result = hasher_.VerifyHashedPassword(null, hashpassword, providedPassword);

            return result == PasswordVerificationResult.Success;
        }
    }
}
