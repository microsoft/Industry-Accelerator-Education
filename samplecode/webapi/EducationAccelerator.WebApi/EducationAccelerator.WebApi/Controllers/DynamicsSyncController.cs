using EducationAccelerator.Models;
using EducationAccelerator.WebApi.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Linq;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EducationAccelerator.Controllers
{
    [Route("sync")]
    public class DynamicsSyncController : BaseController
    {
        public DynamicsSyncController(ApiContext _db, IOptions<CrmConnectionSettings> crmConnection)
            : base(_db, crmConnection)
        {

        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            using (var client = _client)
            {
                var response = client.GetAsync(BuildUrl<CrmConfiguration>());
                var decoded = response.Result.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<ODataResponse<CrmConfiguration>>(decoded);

                if (result.Value == null || result.Value.Count == 0)
                {
                    throw new Exception("Could not locate a valid configuration record in the target Dynamics 365 Organization.");
                }

                var configuration = result.Value.First();

                if (string.IsNullOrEmpty(configuration?.msk12_apiurl))
                {
                    throw new Exception("Dynamics 365 OneRoster configuration record invalid or not found.");
                }

                var agent = new DynamicsSyncAgent(configuration, client, CrmBaseUrl);

                agent.Sync();

                return JsonOk(null);
            }
        }
    }
}
