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
    [Route("ims/oneroster/v1p1/courses")]
    public class CoursesController : BaseController
    {
        public CoursesController(ApiContext _db, IOptions<CrmConnectionSettings> crmConnection) : base(_db, crmConnection)
        {

        }

        // GET ims/oneroster/v1p1/courses
        [HttpGet]
        public IActionResult GetAllCourses()
        {
            using (var client = _client)
            {
                var response = client.GetAsync(BuildUrl<Course>());
                var decoded = response.Result.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<ODataResponse<CrmCourse>>(decoded);

                if (result.Value == null)
                {
                    return NotFound();
                }

                var courses = result.Value.Select(x => new Course(x));

                serializer = new OneRosterSerializer("courses");
                serializer.writer.WriteStartArray();
                foreach (var course in courses)
                {
                    course.AsJson(serializer.writer, BaseUrl());
                }
                serializer.writer.WriteEndArray();

                return JsonOk(FinishSerialization(), courses.Count());
            }
        }

        // GET ims/oneroster/v1p1/courses/5
        [HttpGet("{id}")]
        public IActionResult GetCourse([FromRoute] string id)
        {
            return NotFound();
        }

        // GET ims/oneroster/v1p1/courses/{id}/classes
        [HttpGet("{id}/classes")]
        public IActionResult GetClassesForCourse([FromRoute] string id)
        {
            return NotFound();
        }

        // GET ims/oneroster/v1p1/courses/5/resources
        [HttpGet("{courseId}/resources")]
        public IActionResult GetResourcesForCourse([FromRoute] string courseId)
        {
            return NotFound();
        }
    }
}
