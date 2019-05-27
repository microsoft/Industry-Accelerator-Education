/*
 * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
* See LICENSE in the project root for license information.
*/

using EducationAccelerator.Models;
using Microsoft.AspNetCore.Mvc;

namespace EducationAccelerator.Controllers
{
    [Route("ims/oneroster/v1p1/gradingPeriods")]
    public class GradingPeriodsController : BaseController
    {
        public GradingPeriodsController(ApiContext _db) : base(_db)
        {

        }

        // GET ims/oneroster/v1p1/gradingPeriods
        [HttpGet]
        public IActionResult GetAllGradingPeriods()
        {
            return NotFound();
        }

        // GET ims/oneroster/v1p1/gradingPeriods/5
        [HttpGet("{id}")]
        public IActionResult GetGradingPeriod(string id)
        {
            return NotFound();
        }
    }
}
