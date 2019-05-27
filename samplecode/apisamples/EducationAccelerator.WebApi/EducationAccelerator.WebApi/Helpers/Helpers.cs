using EducationAccelerator.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;

namespace EducationAccelerator.WebApi.Helpers
{
    public class Helpers
    {
        public static HttpClient GetHttpClientWithToken(CrmConnectionSettings settings)
        {
            string token = null;

            using (var authClient = new HttpClient())
            {
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "client_credentials"),
                    new KeyValuePair<string, string>("client_id", settings.ClientId),
                    new KeyValuePair<string, string>("client_secret", settings.ClientSecret),
                    new KeyValuePair<string, string>("resource", settings.Resource),
                });

                var response = authClient.PostAsync(settings.Authority, content);
                var decoded = response.Result.Content.ReadAsStringAsync().Result;
                var authResponse = JsonConvert.DeserializeObject<AuthResponse>(decoded);

                token = authResponse.access_token;
            }

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            return client;
        }

        public class AuthResponse
        {
            public string access_token { get; set; }
        }
    }
}
