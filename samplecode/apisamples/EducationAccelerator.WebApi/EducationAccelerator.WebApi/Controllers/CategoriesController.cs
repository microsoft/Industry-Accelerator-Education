/*
 * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
* See LICENSE in the project root for license information.
*/

using EducationAccelerator.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace EducationAccelerator.Controllers
{
    [Route("ims/oneroster/v1p1/categories")]
    public class CategoriesController : BaseController
    {
        public CategoriesController(ApiContext _db, IOptions<CrmConnectionSettings> crmConnection) : base(_db, crmConnection)
        {
        }

        // GET ims/oneroster/v1p1/categories
        [HttpGet]
        public IActionResult GetAllCategories()
        {
            return NotFound();
        }

        // GET ims/oneroster/v1p1/categories/5
        [HttpGet("{id}")]
        public IActionResult GetCategory([FromRoute] string id)
        {
            return NotFound();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCategory([FromRoute] string id)
        {
            return NotFound();
        }

        [HttpPut("{id}")]
        public IActionResult PutCategory([FromRoute] string id)
        {
            return NotFound();
        }
    }
}
