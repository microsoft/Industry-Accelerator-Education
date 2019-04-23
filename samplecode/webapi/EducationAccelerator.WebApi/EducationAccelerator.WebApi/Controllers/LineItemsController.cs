/*
 * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
* See LICENSE in the project root for license information.
*/

using EducationAccelerator.Models;
using Microsoft.AspNetCore.Mvc;

namespace EducationAccelerator.Controllers
{
    [Route("ims/oneroster/v1p1/lineItems")]
    public class LineItemsController : BaseController
    {
        public LineItemsController(ApiContext _db) : base(_db)
        {
        }

        // GET ims/oneroster/v1p1/lineItems
        [HttpGet]
        public IActionResult GetAllLineItems()
        {
            return NotFound();
        }

        // GET ims/oneroster/v1p1/lineItems/5
        [HttpGet("{id}")]
        public IActionResult GetLineItem([FromRoute] string id)
        {
            return NotFound();
        }

        // DELETE ims/oneroster/v1p1/lineItems/5
        [HttpDelete("{id}")]
        public IActionResult DeleteLineItem([FromRoute] string id)
        {
            return NotFound();
        }

        // PUT ims/oneroster/v1p1/lineItems/5
        [HttpPut("{id}")]
        public IActionResult PutLineItem([FromRoute] string id)
        {
            return NotFound();
        }
    }
}