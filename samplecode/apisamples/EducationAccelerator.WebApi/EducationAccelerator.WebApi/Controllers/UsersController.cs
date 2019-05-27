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
    [Route("ims/oneroster/v1p1/users")]
    public class UsersController : BaseController
    {
        public UsersController(ApiContext _db, IOptions<CrmConnectionSettings> crmConnection) : base(_db, crmConnection)
        {
        }

        // GET ims/oneroster/v1p1/users
        [HttpGet]
        public IActionResult GetAllUsers()
        {
            using (var client = _client)
            {
                var response = client.GetAsync(BuildUrl<User>());
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

        // GET ims/oneroster/v1p1/users/5
        [HttpGet("{id}")]
        public IActionResult GetUser([FromRoute] string id)
        {
            using (var client = _client)
            {
                var response = client.GetAsync(BuildSingleRecordUrl<User>(id));
                var decoded = response.Result.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<CrmUser>(decoded);

                if (result == null)
                {
                    return NotFound();
                }

                var user = new User(result);

                serializer = new OneRosterSerializer("user");
                user.AsJson(serializer.writer, BaseUrl());
                return JsonOk(serializer.Finish());
            }
        }

        // GET ims/oneroster/v1p1/users/5/classes
        [HttpGet("{id}/classes")]
        public IActionResult GetClassesForUser([FromRoute] string id)
        {
            return NotFound();
        }
    }
}
