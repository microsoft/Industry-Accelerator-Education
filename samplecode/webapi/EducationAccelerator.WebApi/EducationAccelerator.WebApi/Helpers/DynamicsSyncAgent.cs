using EducationAccelerator.Middlewares;
using EducationAccelerator.Models;
using EducationAccelerator.WebApi.Serializers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace EducationAccelerator.WebApi.Helpers
{
    public class DynamicsSyncAgent
    {
        private const int PAGE_SIZE = 100;
        private const string ONEROSTER_PATH = "/ims/oneroster/v1p1";

        private string _crmApiUrl;

        private string _oneRosterApiUrl;
        private string _oneRosterKey;
        private string _oneRosterSecret;
        private DateTime? _lastSyncDate;
        private Guid _configurationId;

        private HttpClient _crmClient;
        private HttpClient _oneRosterClient;

        public DynamicsSyncAgent(CrmConfiguration configuration, HttpClient crmClient, string crmBaseUrl)
        {
            _configurationId = configuration.msk12_configurationid;
            _oneRosterApiUrl = configuration.msk12_apiurl;
            _oneRosterKey = configuration.msk12_consumerkey;
            _oneRosterSecret = configuration.msk12_consumersecret;
            _lastSyncDate = configuration.msk12_lastsyncdate;

            _crmClient = crmClient;
            _crmApiUrl = crmBaseUrl;
        }

        private HttpClient GetOneRosterClient()
        {
            string token = null;

            using (var authClient = new HttpClient())
            {
                var authorizationComponents = new Dictionary<string, string>()
                {
                    { "oauth_consumer_key", _oneRosterKey },
                    { "oauth_signature_method", "HMAC-SHA1" },
                    { "oauth_timestamp", ((int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds).ToString() },
                    { "oauth_nonce", Guid.NewGuid().ToString("N") },
                    { "oauth_version", "1.0" }
                };

                authorizationComponents.Add("oauth_signature", GenerateSignature(authorizationComponents));

                var response = authClient.GetAsync(GenerateUrl(authorizationComponents));
                var decoded = response.Result.Content.ReadAsStringAsync().Result;
                var authResponse = JsonConvert.DeserializeObject<AuthResponse>(decoded);

                token = authResponse.access_token;
            }

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            client.BaseAddress = new Uri(_oneRosterApiUrl);

            return client;
        }

        private string GenerateSignature(Dictionary<string, string> authorizationComponents)
        {
            var normalizedParams = OAuth.NormalizeParams(authorizationComponents.ToList());
            var url = $"{_oneRosterApiUrl}/token";
            var signatureBaseString = $"GET&{Uri.EscapeDataString(url)}&{normalizedParams}";

            return OAuth.GenerateHmac(signatureBaseString, "HMAC-SHA1", _oneRosterSecret);
        }

        private string GenerateUrl(Dictionary<string, string> data)
        {
            var components = string.Join(
                '&',
                data
                    .Union(data)
                    .Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value))
            );

            return $"{_oneRosterApiUrl}/token?{components}";
        }

        public void Sync()
        {
            using (_oneRosterClient = GetOneRosterClient())
            {
                var schools = GetAll<Org>("schools?filter=status='active'");
                SaveBatch(schools, o => new CrmOrg(o));

                SaveAll("academicSessions", (AcademicSession a) => new CrmAcademicSession(a));

                foreach (var school in schools)
                {
                    SaveAll($"schools/{school.Id}/classes?filter=status='active'", (IMSClass c) => new CrmClass(c));
                    SaveAll($"schools/{school.Id}/students?filter=status='active'", (User u) => new CrmUser(u));
                    SaveAll($"schools/{school.Id}/teachers?filter=status='active'", (User u) => new CrmUser(u));
                    SaveAll($"schools/{school.Id}/enrollments?filter=status='active' AND role='student'", (Enrollment e) => new CrmEnrollment(e));
                    SaveAll($"schools/{school.Id}/enrollments?filter=status='active' AND role='teacher'", (Enrollment e) => new CrmEnrollment(e));
                }
            }

            UpdateLastSyncDate();
        }

        private List<T> GetAll<T>(string route)
            where T : BaseModel
        {
            var result = new List<T>();
            int pageNumber = 0;

            List<T> page;

            route = ApplyDateFilter(route);

            do
            {
                page = GetPage<T>(route, pageNumber);

                if (page?.Count > 0)
                {
                    result.AddRange(page);
                }

                pageNumber++;
            } while (page?.Count > 0 && page?.Count >= PAGE_SIZE);

            return result;
        }

        private void SaveAll<T, D>(string sourceRoute, Func<T, D> mappingFunction)
            where T : BaseModel
            where D : CrmBaseModel
        {
            int pageNumber = 0;

            List<T> page;

            sourceRoute = ApplyDateFilter(sourceRoute);

            do
            {
                page = GetPage<T>(sourceRoute, pageNumber);

                if (page?.Count > 0)
                {
                    SaveBatch(page, mappingFunction);
                }

                pageNumber++;
            } while (page?.Count > 0 && page?.Count >= PAGE_SIZE);
        }

        private string ApplyDateFilter(string route)
        {
            if (_lastSyncDate != null)
            {
                route += route.IndexOf("?filter") == -1 ? $"?filter=dateLastModified>'{_lastSyncDate.Value.ToString("o")}'" :
                    $" AND dateLastModified>'{_lastSyncDate.Value.ToString("o")}'";
            }

            return route;
        }

        private List<T> GetPage<T>(string route, int page)
        {
            route += route.IndexOf('?') == -1 ? "?" : "&";

            var url = $"{ONEROSTER_PATH}/{route}limit={PAGE_SIZE}&offset={page * PAGE_SIZE}";

            var response = _oneRosterClient.GetAsync(url);
            var json = response.Result.Content.ReadAsStringAsync().Result;
            var result = OneRosterDeserializer.Deserialize<T>(json);

            return result;
        }

        private void SaveBatch<T, D>(List<T> records, Func<T, D> mappingFunction)
            where T : BaseModel
            where D : CrmBaseModel
        {
            foreach (var record in records)
            {
                // Convert object to destination model
                var converted = mappingFunction(record);

                if (converted?.msk12_sourcedid == null)
                {
                    throw new Exception("Encountered record without a sourcedid. Cannot continue.");
                }

                var result = converted.ToJson();

                // Upsert
                var request = new HttpRequestMessage(new HttpMethod("Patch"), new Uri($"{_crmApiUrl}{converted.EntitySetName}(msk12_sourcedid='{converted.msk12_sourcedid}')"));
                request.Headers.Add("OData-MaxVersion", "4.0");
                request.Headers.Add("OData-Version", "4.0");
                request.Content = new StringContent(result, Encoding.UTF8, "application/json");

                var response = _crmClient.SendAsync(request);
                
                if (response.Result.StatusCode != System.Net.HttpStatusCode.OK &&
                    response.Result.StatusCode != System.Net.HttpStatusCode.NoContent)
                {
                    throw new Exception(response.Result.ToString());
                }
            }
        }

        private void UpdateLastSyncDate()
        {
            var record = new CrmConfiguration
            {
                msk12_configurationid = _configurationId,
                msk12_lastsyncdate = DateTime.UtcNow
            };

            var request = new HttpRequestMessage(new HttpMethod("Patch"), new Uri($"{_crmApiUrl}{CrmConfiguration.EntitySetName}({record.msk12_configurationid})"));
            request.Headers.Add("OData-MaxVersion", "4.0");
            request.Headers.Add("OData-Version", "4.0");
            request.Headers.Add("If-Match", "*");
            request.Content = new StringContent(record.ToJson(), Encoding.UTF8, "application/json");

            var response = _crmClient.SendAsync(request);

            if (response.Result.StatusCode != System.Net.HttpStatusCode.OK &&
                response.Result.StatusCode != System.Net.HttpStatusCode.NoContent)
            {
                throw new Exception(response.Result.ToString());
            }
        }
    }
}
