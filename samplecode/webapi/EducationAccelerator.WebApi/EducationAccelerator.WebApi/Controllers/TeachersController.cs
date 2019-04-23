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
    [Route("ims/oneroster/v1p1/teachers")]
    public class TeachersController : BaseController
    {
        internal readonly string[] Filters = { "role='teacher'" };
        public TeachersController(ApiContext _db, IOptions<CrmConnectionSettings> crmConnection) : base(_db, crmConnection)
        {

        }

        // GET ims/oneroster/v1p1/teachers
        [HttpGet]
        public IActionResult GetAllTeachers()
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

        // GET ims/oneroster/v1p1/teachers/5
        [HttpGet("{id}")]
        public IActionResult GetTeacher([FromRoute] string id)
        {
            using (var client = _client)
            {
                var response = client.GetAsync(BuildSingleRecordUrl<User>(id));
                var decoded = response.Result.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<CrmUser>(decoded);

                if (result == null || result.msk12_role != RoleType.teacher)
                {
                    return NotFound();
                }

                var school = new User(result);

                serializer = new OneRosterSerializer("user");
                school.AsJson(serializer.writer, BaseUrl());
                return JsonOk(serializer.Finish());
            }
        }

        // GET ims/oneroster/v1p1/teachers/5/classes
        [HttpGet("{id}/classes")]
        public IActionResult GetClassesForTeacher([FromRoute] string id)
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
                        Filters = new string[] { "role='teacher'", $"user='{{{id}}}'"}
                    }
                };

                var response = client.GetAsync(BuildUrl<IMSClass>(null, links));
                var decoded = response.Result.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<ODataResponse<CrmEnrollment>>(decoded);

                if (result.Value == null)
                {
                    return NotFound();
                }

                var classes = result.Value.Select(x => new Enrollment(x));

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
