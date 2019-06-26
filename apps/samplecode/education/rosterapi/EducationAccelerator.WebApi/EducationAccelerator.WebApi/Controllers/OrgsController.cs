/*
 * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
* See LICENSE in the project root for license information.
*/

using EducationAccelerator.Models;
using Microsoft.AspNetCore.Mvc;

namespace EducationAccelerator.Controllers
{
    [Route("ims/oneroster/v1p1/orgs")]
    public class OrgsController : BaseController
    {
        public OrgsController(ApiContext _db) : base(_db)
        {

        }

        // GET ims/oneroster/v1p1/orgs
        [HttpGet]
        public IActionResult GetAllOrgs()
        {
            return NotFound();
        }

        // GET ims/oneroster/v1p1/orgs/5
        [HttpGet("{id}")]
        public IActionResult GetOrg([FromRoute] string id)
        {
            return NotFound();
        }
    }
}
