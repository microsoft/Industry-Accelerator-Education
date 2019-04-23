using EducationAccelerator.Models;
using Microsoft.AspNetCore.Mvc;

namespace EducationAccelerator.Controllers
{
    [Route("ims/oneroster/v1p1/demographics")]
    public class DemographicsController : BaseController
    {
        public DemographicsController(ApiContext _db) : base(_db)
        {
        }

        // GET ims/oneroster/v1p1/demographics
        [HttpGet]
        public IActionResult GetAllDemographics()
        {
            return NotFound();
        }

        // GET ims/oneroster/v1p1/demographics/5
        [HttpGet("{user_id}")]
        public IActionResult GetDemographic([FromRoute] string user_id)
        {
            return NotFound();
        }
    }
}