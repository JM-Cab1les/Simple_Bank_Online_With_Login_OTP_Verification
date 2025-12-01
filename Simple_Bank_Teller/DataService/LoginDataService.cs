using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Simple_Bank_Teller.DbContexts;
using Simple_Bank_Teller.Models;
using Simple_Bank_Teller.Helper;

namespace Simple_Bank_Teller.DataService
{
    public class LoginDataService
    {
        private readonly BankTeller_DBContext dbContexts_;

        public LoginDataService(BankTeller_DBContext dBContext)
        {
            dbContexts_ = dBContext;
        }


        public async Task<Account> LoginAccount(string userName, string password)
        {
            try
            {
                var existingAccount = await dbContexts_.Accounts.FirstOrDefaultAsync(acc => acc.UserName == userName);

                if (existingAccount == null)
                    return null;

                var verifyPassword = PasswordHash.VerifyPassword(existingAccount.Password, password);

                return verifyPassword ? existingAccount : null;
            }
            catch(Exception ex)
            {
                return null;

            }
        }
    }
}
