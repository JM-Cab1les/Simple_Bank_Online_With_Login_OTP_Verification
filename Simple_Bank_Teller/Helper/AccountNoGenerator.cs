namespace Simple_Bank_Teller.Helper
{
    public class AccountNoGenerator
    {
        private static readonly Random random = new Random();


        public static long GenerateAccountNumber()
        {
            int part1 = random.Next(1000, 10000);   // 4 digits
            int part2 = random.Next(100, 1000);     // 3 digits
            int part3 = random.Next(100, 1000);     // 3 digits

            string numberStr =  $"{part1}{part2}{part3}";
            return Convert.ToInt64(numberStr);

        }
    }
}
