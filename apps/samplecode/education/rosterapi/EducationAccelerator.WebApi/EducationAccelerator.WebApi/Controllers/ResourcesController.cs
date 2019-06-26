using EducationAccelerator.Models;
using Microsoft.AspNetCore.Mvc;

namespace EducationAccelerator.Controllers
{
    [Route("ims/oneroster/v1p1/resources")]
    public class ResourcesController : BaseController
    {
        public ResourcesController(ApiContext _db) : base(_db)
        {
        }

        // GET ims/oneroster/v1p1/resources
        [HttpGet]
        public IActionResult GetAllResources()
        {
            return NotFound();
        }

        // GET ims/oneroster/v1p1/resources/5
        [HttpGet("{id}")]
        public IActionResult GetResource([FromRoute] string id)
        {
            return NotFound();
        }
    }
}