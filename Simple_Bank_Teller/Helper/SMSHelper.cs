using System.Text;
using System.Text.Json;

namespace Simple_Bank_Teller.Helper
{
    public class SMSHelper
    {
        private static readonly string apiToken = "3e4f012fa6494146173bc71d57ac5f991d9295b8";
        private static readonly string baseUrl = "https://www.iprogsms.com/api/v1/otp/send_otp";


        public static async Task<bool> SendSmsAsync(string phoneNumber, string message)

        {
            using var client = new HttpClient();

            var payload = new
            {
                api_token = apiToken,
                phone_number = phoneNumber,
                message = message
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var response = await client.PostAsync(baseUrl, content);

            return response.IsSuccessStatusCode;
        }
    }
}
