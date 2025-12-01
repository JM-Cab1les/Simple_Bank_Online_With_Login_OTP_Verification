using Microsoft.EntityFrameworkCore;
using Simple_Bank_Teller.DbContexts;
using Simple_Bank_Teller.Helper;
using Simple_Bank_Teller.Models;

namespace Simple_Bank_Teller.DataService
{
    public class AccountDataService
    {
        private readonly BankTeller_DBContext dBContext_;

        public AccountDataService(BankTeller_DBContext dBContext)
        {
            dBContext_ = dBContext;
        }


        public async Task <int> CreateAccount(Account model)
        {
            try
            {
                var passwordHash = PasswordHash.HashPassword(model.Password);

                var newRegisterAccount = new Account
                {
                    UserName = model.UserName,
                    Password = passwordHash,
                    AccountInformationId = model.AccountInformationId,
                    AccountNo = model.AccountNo,
                    AccountType = model.AccountType,
                    DateCreated = DateTime.Now,
                    Timestamp = DateTime.Now,

                };

                 dBContext_.Accounts.AddAsync(newRegisterAccount);
                 await dBContext_.SaveChangesAsync();
                return (int)newRegisterAccount.Id;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public async Task<Account> LoginAccount(string userName, string password)
        {
            try
            {
                var existingUser = await dBContext_.Accounts.FirstOrDefaultAsync(username => username.UserName == userName);

                if (existingUser == null)
                    return null;

                var verifyPassword = PasswordHash.VerifyPassword(existingUser.Password, password);

                return verifyPassword ? existingUser : null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }



        public async Task<int> CreateAccountInformation(AccountInformation model)
        {

            try
            {
                model.DateCreated = DateTime.Now;
                model.Timestamp = DateTime.Now;
                dBContext_.AccountInformations.AddAsync(model);
                await dBContext_.SaveChangesAsync();
                return (int)model.Id;
            }
            catch(Exception ex)
            {
                return 0;
            }

        }

        public async Task<Account> GetAccount(long accountID)
        {
            return await dBContext_.Accounts.FirstOrDefaultAsync(account => account.AccountInformationId == accountID);
        }

        public async Task<AccountInformation> GetAccountInformation(long id)
        {
            return await dBContext_.AccountInformations.FirstOrDefaultAsync(acc => acc.Id == id);
        }

        public async Task<MembershipType> GetMembershipType(int id)
        {
            return await dBContext_.MembershipTypes.FirstOrDefaultAsync(mt => mt.Id == id);
        }

        public async Task<Transaction> GetAccountTransaction(long accountID)
        {
            return await dBContext_.Transactions.FirstOrDefaultAsync(tr => tr.AccountId == accountID);
        }


        public async Task<int> StartTransaction(Transaction model)
        {
            decimal currentTotal;
            var existingTransaction = await dBContext_.Transactions.FirstOrDefaultAsync(tr => tr.AccountId == model.AccountId);

            if (existingTransaction == null)
            {
                if(model.TransactionType == 2)
                {
                    throw new InvalidOperationException("Insufficient funds. Account has no balance yet.");
                }


                model.CreatedDate = DateTime.Now;
                model.Timestamp = DateTime.Now;
                dBContext_.AddAsync(model);

            }
            else
            {
                decimal currentBalance = Convert.ToDecimal(existingTransaction.CurrentBal);
                decimal requestAmount = Convert.ToDecimal(model.CurrentBal);

                if (model.TransactionType == 1)
                {
                    currentBalance += requestAmount;

                }
                else
                {
                   if(requestAmount > currentBalance)
                    {
                        throw new InvalidOperationException("Insuffient funds. Available balance is not enough.");
                    }

                    currentBalance -= requestAmount;
                }

                existingTransaction.CurrentBal = currentBalance.ToString();
                existingTransaction.AvailableBal = currentBalance.ToString();
                existingTransaction.TransactionType = model.TransactionType;
                existingTransaction.Timestamp = DateTime.Now;
            }

            return await dBContext_.SaveChangesAsync();
        }
    }
}

