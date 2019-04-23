using EducationAccelerator.Models;
using Microsoft.AspNetCore.Mvc;
using System;

namespace EducationAccelerator.Controllers
{
    [Route("token")]
    public class TokenController : BaseController
    {
        public TokenController(ApiContext _db) : base(_db)
        {
        }

        [HttpGet]
        public IActionResult GetToken()
        {
            var verification = Middlewares.OAuth.Verify(HttpContext, db);
            if (verification != 0)
            {
                return StatusCode(verification);
            }
            string token = Middlewares.OAuth.GenerateBearerToken();
            var tokenEntry = new OauthToken()
            {
                Value = token,
                CreatedAt = DateTime.Now
            };

            db.OauthTokens.Add(tokenEntry);
            db.SaveChanges();
            return JsonOk($"{{ \"access_token\": \"{token}\" }}");
        }
    }
}