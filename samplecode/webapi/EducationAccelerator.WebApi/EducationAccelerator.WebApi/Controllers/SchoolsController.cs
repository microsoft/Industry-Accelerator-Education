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
    [Route("ims/oneroster/v1p1/schools")]
    public class SchoolsController : BaseController
    {
        internal readonly string[] Filters = { "type='school'" };
        public SchoolsController(ApiContext _db, IOptions<CrmConnectionSettings> crmConnection) : base(_db, crmConnection)
        {
        }

        // GET ims/oneroster/v1p1/schools
        [HttpGet]
        public IActionResult GetAllSchools()
        {
            using (var client = _client)
            {
                var response = client.GetAsync(BuildUrl<Org>(Filters));
                var decoded = response.Result.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<ODataResponse<CrmOrg>>(decoded);

                if (result.Value == null)
                {
                    return NotFound();
                }

                var schools = result.Value.Select(x => new Org(x));

                serializer = new OneRosterSerializer("orgs");
                serializer.writer.WriteStartArray();
                foreach (var school in schools)
                {
                    school.AsJson(serializer.writer, BaseUrl());
                }
                serializer.writer.WriteEndArray();

                return JsonOk(FinishSerialization(), schools.Count());
            }
        }

        // GET ims/oneroster/v1p1/schools/5
        [HttpGet("{id}")]
        public IActionResult GetSchool([FromRoute] string id)
        {
            using (var client = _client)
            {
                var response = client.GetAsync(BuildSingleRecordUrl<Org>(id));
                var decoded = response.Result.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<CrmOrg>(decoded);

                if (result == null || result.msk12_accounttype != OrgType.school)
                {
                    return NotFound();
                }

                var school = new Org(result);

                serializer = new OneRosterSerializer("org");
                school.AsJson(serializer.writer, BaseUrl());
                return JsonOk(serializer.Finish());
            }
        }

        // GET schools/{id}/courses
        [HttpGet("{id}/courses")]
        public IActionResult GetCoursesForSchool([FromRoute] string id)
        {
            return NotFound();
        }

        // GET ims/oneroster/v1p1/schools/{id}/enrollments
        [HttpGet("{id}/enrollments")]
        public IActionResult GetEnrollmentsForSchool([FromRoute] string id)
        {
            using (var client = _client)
            {
                var response = client.GetAsync(BuildUrl<Enrollment>(new string[] { $"school='{{{id}}}'"}));
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

        // GET ims/oneroster/v1p1/schools/{id}/classes
        [HttpGet("{id}/classes")]
        public IActionResult GetClassesForSchool([FromRoute] string id)
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
                        Filters = new string[] { $"school='{{{id}}}'"}
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

        // GET ims/oneroster/v1p1/schools/{id}/students
        [HttpGet("{id}/students")]
        public IActionResult GetStudentsForSchool([FromRoute] string id)
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
                        Filters = new string[] { $"role='student' AND school='{{{id}}}'"}
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

        // GET ims/oneroster/v1p1/schools/{id}/teachers
        [HttpGet("{id}/teachers")]
        public IActionResult GetTeachersForSchool([FromRoute] string id)
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
                        Filters = new string[] { $"role='teacher' AND school='{{{id}}}'"}
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

        // GET ims/oneroster/v1p1/schools/{id}/terms
        [HttpGet("{id}/terms")]
        public IActionResult GetTermsForSchool([FromRoute] string id)
        {
            return NotFound();
        }

        // GET ims/oneroster/v1p1/schools/{school_id}/classes/class_id}/enrollments
        [HttpGet("{schoolId}/classes/{classId}/enrollments")]
        public IActionResult GetEnrollmentsForClassInSchool([FromRoute] string schoolId, string classId)
        {
            return NotFound();
        }

        // GET ims/oneroster/v1p1/schools/{school_id}/classes/{class_id}/students
        [HttpGet("{schoolId}/classes/{classId}/students")]
        public IActionResult GetStudentsForClassInSchool([FromRoute] string schoolId, string classId)
        {
            return NotFound();
        }

        // GET ims/oneroster/v1p1/schools/{school_id}/classes/{class_id}/teachers
        [HttpGet("{schoolId}/classes/{classId}/teachers")]
        public IActionResult GetTeachersForClassInSchool([FromRoute] string schoolId, string classId)
        {
            return NotFound();
        }
    }
}
