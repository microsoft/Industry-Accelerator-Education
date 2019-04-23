/*
 * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
* See LICENSE in the project root for license information.
*/

using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Sds
{
    public class SdsManager
    {
        private static readonly HttpClient Client = new HttpClient();
        private static string profileUrl = "beta/education/synchronizationProfiles";
        private string _token;

        public SdsManager(string token)
        {
            _token = token;
        }

        public async Task<HttpResponseMessage> QueryProfileAsync(string profileId)
        {
            return await QueryGraphAsync(profileUrl + $"/{profileId}");
        }

        public async Task<HttpResponseMessage> PostProfileAsync(string requestBody)
        {
            return await PostGraphAsync(profileUrl, requestBody);
        }

        public async Task<HttpResponseMessage> QueryAllProfilesAsync()
        {
            return await QueryGraphAsync(profileUrl);
        }

        public async Task<HttpResponseMessage> QuerySkusAsync()
        {
            return await QueryGraphAsync("/v1.0/subscribedSkus");
        }

        private async Task<HttpResponseMessage> PostGraphAsync(string relativeUrl, string requestBody)
        {
            var req = new HttpRequestMessage(HttpMethod.Post, $"https://graph.microsoft.com/{relativeUrl}")
            {
                Content = new StringContent(requestBody, Encoding.UTF8, "application/json")
            };
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            return await Client.SendAsync(req);
        }

        private async Task<HttpResponseMessage> QueryGraphAsync(string relativeUrl)
        {
            var req = new HttpRequestMessage(HttpMethod.Get, $"https://graph.microsoft.com/{relativeUrl}");
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            return await Client.SendAsync(req);
        }

        public async Task StartCsvSync(string profileId)
        {
            var req = new HttpRequestMessage(HttpMethod.Post, $"https://graph.microsoft.com/testsds/synchronizationProfiles/{profileId}/start");
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer");
            var response = await Client.SendAsync(req);
            var responseText = await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetCsvUploadUrl(string profileId)
        {
            var req = new HttpRequestMessage(HttpMethod.Get, $"https://graph.microsoft.com/{profileUrl}/{profileId}/uploadUrl");
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            var res = await Client.SendAsync(req);
            var parsed = JObject.Parse(await res.Content.ReadAsStringAsync());
            return (string)parsed["value"];
        }
    }
}
