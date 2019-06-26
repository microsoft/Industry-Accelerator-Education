/*
 * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
* See LICENSE in the project root for license information.
*/

using EducationAccelerator.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace EducationAccelerator.Controllers
{
    [Route("ims/oneroster/v1p1/enrollments")]
    public class EnrollmentsController : BaseController
    {
        public EnrollmentsController(ApiContext _db, IOptions<CrmConnectionSettings> crmConnection) : base(_db, crmConnection)
        {

        }

        // GET ims/oneroster/v1p1/enrollments
        [HttpGet]
        public IActionResult GetAllEnrollments()
        {
            return NotFound();
        }

        // GET ims/oneroster/v1p1/enrollments/5
        [HttpGet("{id}")]
        public IActionResult GetEnrollment([FromRoute] string id)
        {
            return NotFound();
        }
    }
}
