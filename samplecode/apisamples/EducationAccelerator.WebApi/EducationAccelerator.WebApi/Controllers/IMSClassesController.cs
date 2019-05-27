/*
 * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
* See LICENSE in the project root for license information.
*/

using EducationAccelerator.Models;
using EducationAccelerator.Serializers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace EducationAccelerator.Controllers
{
    [Route("ims/oneroster/v1p1/classes")]
    public class IMSClassesController : BaseController
    {
        public IMSClassesController(ApiContext _db, IOptions<CrmConnectionSettings> crmConnection) : base(_db, crmConnection)
        {
        }

        // GET ims/oneroster/v1p1/classes
        [HttpGet]
        public IActionResult GetAllClasses()
        {
            using (var client = _client)
            {
                var response = client.GetAsync(BuildUrl<IMSClass>());
                var decoded = response.Result.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<ODataResponse<CrmClass>>(decoded);

                if (result.Value == null)
                {
                    return NotFound();
                }

                var sessions = result.Value.Select(x => new IMSClass(x));

                serializer = new OneRosterSerializer("classes");
                serializer.writer.WriteStartArray();
                foreach (var session in sessions)
                {
                    session.AsJson(serializer.writer, BaseUrl());
                }
                serializer.writer.WriteEndArray();

                return JsonOk(FinishSerialization(), sessions.Count());
            }
        }

        // GET ims/oneroster/v1p1/classes/5
        [HttpGet("{id}")]
        public IActionResult GetClass([FromRoute] string id)
        {
            using (var client = _client)
            {
                var response = client.GetAsync(BuildSingleRecordUrl<IMSClass>(id));
                var decoded = response.Result.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<CrmClass>(decoded);

                if (result == null)
                {
                    return NotFound();
                }

                var school = new IMSClass(result);

                serializer = new OneRosterSerializer("class");
                school.AsJson(serializer.writer, BaseUrl());
                return JsonOk(serializer.Finish());
            }
        }

        // GET ims/oneroster/v1p1/classes/5/enrollments
        [HttpGet("{id}/enrollments")]
        public IActionResult GetEnrollmentsForClass([FromRoute] string id)
        {
            using (var client = _client)
            {
                var response = client.GetAsync(BuildUrl<Enrollment>(new string[] { $"class='{{{id}}}'" }));
                var decoded = response.Result.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<ODataResponse<CrmEnrollment>>(decoded);

                if (result.Value == null)
                {
                    return NotFound();
                }

                var enrollments = result.Value.Select(x => new Enrollment(x));

                serializer = new OneRosterSerializer("enrollments");
                serializer.writer.WriteStartArray();
                foreach (var enrollment in enrollments)
                {
                    enrollment.AsJson(serializer.writer, BaseUrl());
                }
                serializer.writer.WriteEndArray();

                return JsonOk(FinishSerialization(), enrollments.Count());
            }
        }

        // GET ims/oneroster/v1p1/classes/5/students
        [HttpGet("{id}/students")]
        public IActionResult GetStudentsForClass([FromRoute] string id)
        {
            using (var client = _client)
            {
                var links = new List<LinkEntityData>() {
                    new LinkEntityData()
                    {
                        EntityName = "msk12_enrollment",
                        EntityType = typeof(Enrollment),
                        JoinMapping = "from='msk12_contact' to='contactid'",
                        Alias = "enrollment",
                        Filters = new string[] { $"role='student' AND class='{{{id}}}'"}
                    }
                };

                var response = client.GetAsync(BuildUrl<User>(null, links));
                var decoded = response.Result.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<ODataResponse<CrmUser>>(decoded);

                if (result.Value == null)
                {
                    return NotFound();
                }

                var users = result.Value.Select(x => new User(x));

                serializer = new OneRosterSerializer("users");
                serializer.writer.WriteStartArray();
                foreach (var user in users)
                {
                    user.AsJson(serializer.writer, BaseUrl());
                }
                serializer.writer.WriteEndArray();

                return JsonOk(FinishSerialization(), users.Count());
            }
        }

        // GET ims/oneroster/v1p1/classes/5/teachers
        [HttpGet("{id}/teachers")]
        public IActionResult GetTeachersForClass([FromRoute] string id)
        {
            using (var client = _client)
            {
                var links = new List<LinkEntityData>() {
                    new LinkEntityData()
                    {
                        EntityName = "msk12_enrollment",
                        EntityType = typeof(Enrollment),
                        JoinMapping = "from='msk12_contact' to='contactid'",
                        Alias = "enrollment",
                        Filters = new string[] { $"role='teacher' AND class='{{{id}}}'"}
                    }
                };

                var response = client.GetAsync(BuildUrl<User>(null, links));
                var decoded = response.Result.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<ODataResponse<CrmUser>>(decoded);

                if (result.Value == null)
                {
                    return NotFound();
                }

                var users = result.Value.Select(x => new User(x));

                serializer = new OneRosterSerializer("users");
                serializer.writer.WriteStartArray();
                foreach (var user in users)
                {
                    user.AsJson(serializer.writer, BaseUrl());
                }
                serializer.writer.WriteEndArray();

                return JsonOk(FinishSerialization(), users.Count());
            }
        }

        // GET ims/oneroster/v1p1/classes/5/lineItems
        [HttpGet("{id}/lineItems")]
        public IActionResult GetLineItemsForClass([FromRoute] string id)
        {
            return NotFound();
        }

        // GET ims/oneroster/v1p1/classes/5/results
        [HttpGet("{id}/results")]
        public IActionResult GetResultsForClass([FromRoute] string id)
        {
            return NotFound();
        }

        // GET ims/oneroster/v1p1/classes/5/lineItems/5/results
        [HttpGet("{id}/lineItems/{lineItemId}/results")]
        public IActionResult GetResultsForLineItemForClass([FromRoute] string id, [FromRoute] string lineItemId)
        {
            return NotFound();
        }

        // GET ims/oneroster/v1p1/classes/5/students/5/results
        [HttpGet("{id}/students/{studentId}/results")]
        public IActionResult GetResultsForStudentForClass([FromRoute] string id, [FromRoute] string studentId)
        {
            return NotFound();
        }

        // GET ims/oneroster/v1p1/classes/5/resources
        [HttpGet("{id}/resources")]
        public IActionResult GetResourcesForClass([FromRoute] string id)
        {
            return NotFound();
        }
    }
}
