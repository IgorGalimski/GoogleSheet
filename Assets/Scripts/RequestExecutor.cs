using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Google.GData.Client;
using Newtonsoft.Json;

namespace DefaultNamespace
{
    public class RequestExecutor
    {
        public static async Task SendRequestAsync(URLBuilder urlBuilder, object value, Action<string> responseHandler)
        {
            await GoogleDataStorage.Instance.RefreshAccessToken();
            
            var httpClient = Utils.HttpClient;
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", GoogleDataStorage.Instance.AccessToken);

            var json = JsonConvert.SerializeObject(value);

            var content = new StringContent(json);

            using (var response = await httpClient.PostAsync(urlBuilder.GetURL(), content))
            {
                if (response.IsSuccessStatusCode)
                {
                    var str = await response.Content.ReadAsStringAsync();
                    
                    responseHandler.Invoke(str);
                }
            }
        }
    }
}