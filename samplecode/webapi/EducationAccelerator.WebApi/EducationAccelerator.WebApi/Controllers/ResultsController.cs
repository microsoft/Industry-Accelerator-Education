/*
 * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
* See LICENSE in the project root for license information.
*/

using EducationAccelerator.Models;
using Microsoft.AspNetCore.Mvc;

namespace EducationAccelerator.Controllers
{
    [Route("ims/oneroster/v1p1/results")]
    public class ResultsController : BaseController
    {
        public ResultsController(ApiContext _db) : base(_db)
        {

        }

        // GET ims/oneroster/v1p1/results
        [HttpGet]
        public IActionResult GetAllResults()
        {
            return NotFound();
        }

        // GET ims/oneroster/v1p1/results/5
        [HttpGet("{id}")]
        public IActionResult GetResult([FromRoute] string id)
        {
            return NotFound();
        }

        // DELETE ims/oneroster/v1p1/results/5
        [HttpDelete("{id}")]
        public IActionResult DeleteResult ([FromRoute] string id)
        {
            return NotFound();
        }

        // PUT ims/oneroster/v1p1/results/5
        [HttpPut("{id}")]
        public IActionResult PutResult([FromRoute] string id)
        {
            return NotFound();
        }
    }
}