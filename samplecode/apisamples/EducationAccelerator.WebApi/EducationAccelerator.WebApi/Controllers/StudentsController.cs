/*
 * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
* See LICENSE in the project root for license information.
*/

using EducationAccelerator.Models;
using EducationAccelerator.Serializers;
using EducationAccelerator.Vocabulary;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace EducationAccelerator.Controllers
{
    [Route("ims/oneroster/v1p1/students")]
    public class StudentsController : BaseController
    {
        internal readonly string[] Filters = new string[] { "role='student'" };

        public StudentsController(ApiContext _db, IOptions<CrmConnectionSettings> crmConnection) : base(_db, crmConnection)
        {

        }

        // GET ims/oneroster/v1p1/students
        [HttpGet]
        public IActionResult GetAllStudents()
        {
            using (var client = _client)
            {
                var response = client.GetAsync(BuildUrl<User>(Filters));
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

        // GET ims/oneroster/v1p1/students/5
        [HttpGet("{id}")]
        public IActionResult GetStudent([FromRoute] string id)
        {
            using (var client = _client)
            {
                var response = client.GetAsync(BuildSingleRecordUrl<User>(id));
                var decoded = response.Result.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<CrmUser>(decoded);

                if (result == null || result.msk12_role != RoleType.student)
                {
                    return NotFound();
                }

                var user = new User(result);

                serializer = new OneRosterSerializer("user");
                user.AsJson(serializer.writer, BaseUrl());
                return JsonOk(serializer.Finish());
            }
        }

        // GET ims/oneroster/v1p1/students/{student_id}/classes
        [HttpGet("{id}/classes")]
        public IActionResult GetClassesForStudent([FromRoute] string id)
        {
            using (var client = _client)
            {
                var links = new List<LinkEntityData>() {
                    new LinkEntityData()
                    {
                        EntityName = "msk12_enrollment",
                        EntityType = typeof(Enrollment),
                        JoinMapping = "from='msk12_classid' to='msk12_classid'",
                        Alias = "enrollment",
                        Filters = new string[] { $"role='student' AND user='{{{id}}}'"}
                    }
                };

                var response = client.GetAsync(BuildUrl<IMSClass>(null, links));
                var decoded = response.Result.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<ODataResponse<CrmClass>>(decoded);

                if (result.Value == null)
                {
                    return NotFound();
                }

                var classes = result.Value.Select(x => new IMSClass(x));

                serializer = new OneRosterSerializer("classes");
                serializer.writer.WriteStartArray();
                foreach (var imsClass in classes)
                {
                    imsClass.AsJson(serializer.writer, BaseUrl());
                }
                serializer.writer.WriteEndArray();

                return JsonOk(FinishSerialization(), classes.Count());
            }
        }
    }
}
