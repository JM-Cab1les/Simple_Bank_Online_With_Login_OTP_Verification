namespace Simple_Bank_Teller.Controllers.Models
{
    public class TransactionModel
    {
        public long Id { get; set; }

        public long AccountId { get; set; }

        public int MembershipType { get; set; }

        public string CurrentBal { get; set; }

        public string AvailableBal { get; set; }

        public int? TransactionType { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime Timestamp { get; set; }

        public string CreatedBy { get; set; }
    }
}
