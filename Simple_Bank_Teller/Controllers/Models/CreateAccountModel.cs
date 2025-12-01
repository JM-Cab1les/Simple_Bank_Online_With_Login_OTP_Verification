namespace Simple_Bank_Teller.Controllers.Models
{
    public class CreateAccountModel
    {
        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public int? Gender { get; set; }

        public string ContactNo { get; set; }

        public string Address { get; set; }

        public string EmailAddress { get; set; }

        public int? AccountType { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }
    }
}
