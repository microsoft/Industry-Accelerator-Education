using EducationAccelerator.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;

namespace EducationAccelerator.Controllers
{
    [Route("seeds")]
    public class SeedsController : BaseController
    {
        public SeedsController(ApiContext _db, IOptions<CrmConnectionSettings> crmConnection = null) : base(_db, crmConnection)
        {
        }

        [HttpGet]
        public IActionResult Reset()
        {
            try
            {
                SeedData.Initialize(db);
            }
            catch (Exception e)
            {
                return Ok(e.ToString());
            }
            return Ok("Seeded");
        }
    }
}
