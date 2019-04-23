/*
 * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
* See LICENSE in the project root for license information.
*/

using EducationAccelerator.Models;
using EducationAccelerator.Serializers;
using Microsoft.AspNetCore.Mvc;

namespace EducationAccelerator.Controllers
{
    public class CsvController : Controller
    {
        private ApiContext db;
        public CsvController(ApiContext _db)
        {
            db = _db;
        }
        [Route("csv/bulk")]
        public void Bulk()
        {
            Response.ContentType = "binary/octet-stream";
            Response.Headers["Content-Disposition"] = "attachment; filename=oneroster.zip";
            var serializer = new CsvSerializer(db);
            serializer.Serialize(Response.Body);
        }
    }
}