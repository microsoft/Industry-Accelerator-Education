/*
 * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
* See LICENSE in the project root for license information.
*/

using EducationAccelerator.Models;
using EducationAccelerator.Serializers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Linq;

namespace EducationAccelerator.Controllers
{
    [Route("ims/oneroster/v1p1/academicSessions")]
    public class AcademicSessionsController : BaseController
    {
        public AcademicSessionsController(ApiContext _db, IOptions<CrmConnectionSettings> crmConnection) : base(_db, crmConnection)
        {
        }

        // GET ims/oneroster/v1p1/academicSessions
        [HttpGet]
        public IActionResult GetAllAcademicSessions()
        {
            using (var client = _client)
            {
                var response = client.GetAsync(BuildUrl<AcademicSession>());
                var decoded = response.Result.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<ODataResponse<CrmAcademicSession>>(decoded);
                
                if (result.Value == null)
                {
                    return NotFound();
                }

                var sessions = result.Value.Select(x => new AcademicSession(x));

                serializer = new OneRosterSerializer("academicSessions");
                serializer.writer.WriteStartArray();
                foreach (var session in sessions)
                {
                    session.AsJson(serializer.writer, BaseUrl());
                }
                serializer.writer.WriteEndArray();

                return JsonOk(FinishSerialization(), sessions.Count());
            }
        }

        // GET ims/oneroster/v1p1/academicSessions/5
        [HttpGet("{id}")]
        public IActionResult GetAcademicSession([FromRoute] string id)
        {
            using (var client = _client)
            {
                var response = client.GetAsync(BuildSingleRecordUrl<AcademicSession>(id));
                var decoded = response.Result.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<CrmAcademicSession>(decoded);

                var session = new AcademicSession(result);

                serializer = new OneRosterSerializer("user");
                session.AsJson(serializer.writer, BaseUrl());
                return JsonOk(serializer.Finish());
            }
        }
    }
}
