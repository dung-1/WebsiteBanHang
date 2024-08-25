using Newtonsoft.Json;
using WebsiteBanHang.Areas.Admin.Common;

namespace WebsiteBanHang.Service
{
    public class VietQRService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public VietQRService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<string> GenerateQRCodeAsync(decimal amount)
        {
            long tongCongInCent = Convert.ToInt64(amount);

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.vietqr.io/v2/generate");
            request.Headers.Add("x-client-id", _configuration["VietQR:ClientId"]);
            request.Headers.Add("x-api-key", _configuration["VietQR:ApiKey"]);

            var payload = new
            {
                accountNo = _configuration["VietQR:AccountNo"],
                accountName = _configuration["VietQR:AccountName"],
                acqId = _configuration["VietQR:BankId"],
                addInfo = _configuration["VietQR:Description"],
                amount = tongCongInCent, 
                template = _configuration["VietQR:Template"]
            };

            request.Content = new StringContent(JsonConvert.SerializeObject(payload), System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<VietQRResponse>(responseContent);

            return result.Data.QrDataURL;
        }


    }

}
