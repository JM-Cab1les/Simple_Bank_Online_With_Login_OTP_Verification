using Microsoft.EntityFrameworkCore;
using Simple_Bank_Teller.DbContexts;
using Simple_Bank_Teller.Models;

namespace Simple_Bank_Teller.DataService
{
    public class MaintenanceDataService
    {
        private readonly BankTeller_DBContext dbContexts_;

        public MaintenanceDataService(BankTeller_DBContext dBContext)
        {
            dbContexts_ = dBContext;
        }

         public async Task<List<GenderType>> GetGenderListAsync()
        {
            return await dbContexts_.GenderTypes.ToListAsync();
        }

        public async Task<List<MembershipType>> GetMembershipTypeListAsync()
        {
            return await dbContexts_.MembershipTypes.ToListAsync();
        }
    }
}
