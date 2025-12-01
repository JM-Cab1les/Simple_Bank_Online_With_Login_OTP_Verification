using Simple_Bank_Teller.Models;

namespace Simple_Bank_Teller.Controllers.Models
{
    public class DashboardModel
    {
       public Account UserAccount { get; set; }
       public AccountInformation UserAccountInformation { get; set; }
       public MembershipType MembersValueType { get; set; }
       public Transaction? TransactionValue { get; set; } = null;


    }


}
