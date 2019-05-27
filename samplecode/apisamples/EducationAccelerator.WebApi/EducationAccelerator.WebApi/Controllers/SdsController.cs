/*
 * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
* See LICENSE in the project root for license information.
*/

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sds;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EducationAccelerator.Controllers
{
    [Authorize]
    [Route("sds")]
    public class SdsController : Controller
    {
        private static readonly HttpClient Client = new HttpClient();
        private readonly IConfiguration _config;
        private SdsManager manager;

        public SdsController(IConfiguration config)
        {
            _config = config;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            ViewBag.url = $"{(Request.IsHttps ? "https" : "http")}://{Request.Host}/ims/oneroster/v1p1";
            return View();
        }

        [Route("csv")]
        public async Task<IActionResult> CsvAsync(List<IFormFile> files)
        {
            if (files.Count < 6)
            {
                return BadRequest();
            }

            if (manager == null)
            {
                manager = new SdsManager(await GetAccessTokenAsync());
            }

            var profile = await GetProfileAsync("csv");
            if (profile["error"] != null)
            {
                var err = profile["error"]["message"];
                ViewBag.Message = $"CSV connector failed with error \"{err}\"";
                ViewBag.Result = "error";
                return View("Csv");
            }

            var profileId = (string)profile["id"];
            var uploadSas = await manager.GetCsvUploadUrl(profileId);

            await UploadToUrl(files, uploadSas);
            await StartCsvSyncSafely(profileId);

            profile = await GetProfileAsync("csv");
            if (profile["error"] != null)
            {
                var err = profile["error"]["message"];
                ViewBag.Message = $"CSV connector failed with error \"{err}\"";
                ViewBag.Result = "error";
                return View("Csv");
            }

            ViewBag.Message = $"CSV connector has id {profile["id"]} and status {profile["state"]}";
            ViewBag.Result = "success";
            return View("Csv");
        }

        [Route("rest")]
        public async Task<IActionResult> RestAsync()
        {
            if (manager == null)
            {
                manager = new SdsManager(await GetAccessTokenAsync());
            }
            var profile = await GetProfileAsync("rest");
            if(profile["error"] != null)
            {
                var err = profile["error"]["message"];
                ViewBag.Message = $"OneRoster REST connector failed with error \"{err}\"";
                ViewBag.Result = "error";
            }
            else
            {
                ViewBag.Message = $"OneRoster REST connector has id {profile["id"]} and status {profile["state"]}";
                ViewBag.Result = "success";
            }
            return View("Rest");
        }

        private async Task<JObject> GetProfileAsync(string profileType)
        {
            // find existing matching profile
            var profileName = $"OneRoster{profileType.ToUpper()}Profile";
            HttpResponseMessage res = await manager.QueryAllProfilesAsync();
            var profiles = (JArray)JObject.Parse(await res.Content.ReadAsStringAsync())["value"];

            foreach (var profile in profiles)
            {
                if ((string)profile["displayName"] == profileName)
                {
                    return (JObject)profile;
                }
            }

            // create new profile
            var generatedProfile = await GenerateProfileAsync(profileType);
            HttpResponseMessage res2 = await manager.PostProfileAsync(generatedProfile);
            return JObject.Parse(await res2.Content.ReadAsStringAsync());
        }

        private async Task UploadToUrl(List<IFormFile> files, string sas)
        {
            CloudBlobContainer container = new CloudBlobContainer(new Uri(sas));

            foreach (var file in files)
            {
                var fileBlob = container.GetBlockBlobReference(file.FileName);
                await fileBlob.UploadFromStreamAsync(file.OpenReadStream());
            }
        }

        private async Task StartCsvSyncSafely(string profileId)
        {
            var profileIsReady = false;
            do
            {
                var res = await manager.QueryProfileAsync(profileId);
                var responseText = await res.Content.ReadAsStringAsync();
                profileIsReady = (string)JObject.Parse(responseText)["state"] == "provisioned";

                if (!profileIsReady)
                {
                    await Task.Delay(5000);
                }
            } while (!profileIsReady);
            await manager.StartCsvSync(profileId);
        }

        private async Task<string> GetAccessTokenAsync()
        {
            return await HttpContext.GetTokenAsync("access_token");
        }

        // https://github.com/OfficeDev/O365-EDU-Tools/blob/master/SDSProfileManagementDocs/api/synchronizationProfile_create.md
        private async Task<string> GenerateProfileAsync(string profileType)
        {
            var skus = await GetOfficeSkusAsync();

            StringBuilder sb = new StringBuilder();
            using (JsonWriter writer = new JsonTextWriter(new StringWriter(sb)))
            {
                writer.WriteStartObject();

                writer.WritePropertyName("displayName");
                writer.WriteValue($"OneRoster{profileType.ToUpper()}Profile");

                writer.WritePropertyName("dataProvider");
                writer.WriteStartObject();

                writer.WritePropertyName("@odata.type");

                if (profileType == "csv")
                {
                    writer.WriteValue("#microsoft.graph.educationCsvDataProvider");
                }
                else
                {
                    writer.WriteValue("#microsoft.graph.educationcsvdataprovider");
                }

                writer.WriteEndObject();

                writer.WritePropertyName("identitySynchronizationConfiguration");
                writer.WriteStartObject();

                writer.WritePropertyName("@odata.type");
                writer.WriteValue("#microsoft.graph.educationidentitycreationconfiguration");

                writer.WritePropertyName("userDomains");
                writer.WriteStartArray();
                writer.WriteStartObject();
                writer.WritePropertyName("appliesTo");
                writer.WriteValue("student");

                writer.WritePropertyName("name");
                writer.WriteValue(_config.GetValue<string>("AzureDomain"));
                writer.WriteEndObject();

                writer.WriteStartObject();
                writer.WritePropertyName("appliesTo");
                writer.WriteValue("teacher");

                writer.WritePropertyName("name");
                writer.WriteValue(_config.GetValue<string>("AzureDomain"));
                writer.WriteEndObject();
                writer.WriteEndArray();

                writer.WriteEndObject();

                writer.WritePropertyName("licensesToAssign");
                writer.WriteStartArray();

                writer.WriteStartObject();
                writer.WritePropertyName("appliesTo");
                writer.WriteValue("teacher");

                writer.WritePropertyName("skuIds");
                writer.WriteStartArray();
                if(skus.Item1 != null)
                    writer.WriteValue(skus.Item1);
                writer.WriteEndArray();
                writer.WriteEndObject();

                writer.WriteStartObject();
                writer.WritePropertyName("appliesTo");
                writer.WriteValue("student");

                writer.WritePropertyName("skuIds");
                writer.WriteStartArray();
                if(skus.Item2 != null)
                    writer.WriteValue(skus.Item2);
                writer.WriteEndArray();
                writer.WriteEndObject();

                writer.WriteEndArray();

                writer.WriteEndObject();

                return sb.ToString();
            }
        }

        // https://developer.microsoft.com/en-us/graph/docs/api-reference/v1.0/api/subscribedsku_list
        private async Task<Tuple<string, string>> GetOfficeSkusAsync()
        {
            HttpResponseMessage res = await manager.QuerySkusAsync();

            var skus = (JArray)JObject.Parse(await res.Content.ReadAsStringAsync())["value"];

            string studentSku = null;
            string teacherSku = null;

            foreach (var sku in skus)
            {
                if ((string)sku["skuPartNumber"] == "STANDARDWOFFPACK_FACULTY")
                {
                    teacherSku = (string)sku["skuId"];
                }
                if ((string)sku["skuPartNumber"] == "STANDARDWOFFPACK_STUDENT")
                {
                    studentSku = (string)sku["skuId"];
                }
            }

            return new Tuple<string, string>(teacherSku, studentSku);
        }
    }
}
