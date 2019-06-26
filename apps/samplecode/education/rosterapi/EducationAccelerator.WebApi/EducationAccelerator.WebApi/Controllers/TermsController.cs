/*
 * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
* See LICENSE in the project root for license information.
*/

using EducationAccelerator.Models;
using Microsoft.AspNetCore.Mvc;

namespace EducationAccelerator.Controllers
{
    [Route("ims/oneroster/v1p1/terms")]
    public class TermsController : BaseController
    {
        public TermsController(ApiContext _db) : base(_db)
        {

        }

        // GET ims/oneroster/v1p1/terms
        [HttpGet]
        public IActionResult GetAllTerms()
        {
            return NotFound();
        }

        // GET ims/oneroster/v1p1/terms/5
        [HttpGet("{id}")]
        public IActionResult GetTerm([FromRoute] string id)
        {
            return NotFound();
        }

        // GET ims/oneroster/v1p1/terms/{id}/classes
        [HttpGet("{id}/classes")]
        public IActionResult GetClassesForTerm([FromRoute] string id)
        {
            return NotFound();
        }

        // GET ims/oneroster/v1p1/terms/{id}/gradingPeriods
        [HttpGet("{id}/gradingPeriods")]
        public IActionResult GetGradingPeriodsForTerm([FromRoute] string id)
        {
            return NotFound();
        }
    }
}
