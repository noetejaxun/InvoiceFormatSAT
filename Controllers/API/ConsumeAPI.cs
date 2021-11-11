using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceFormatSAT.Controllers.API
{
    public class ConsumeAPI
    {
        public async Task<string> consumeApi(string uri, string query, string contentType)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    StringContent content = new StringContent(query, Encoding.UTF8, contentType);

                    using (var response = await httpClient.PostAsync(uri, content))
                    {
                        return await response.Content.ReadAsStringAsync();
                    }
                }
            }
            catch (Exception)
            {
                return "";
            }

        }
    }
}
