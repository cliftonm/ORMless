using System;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Lib;

namespace Demo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PublicController : ControllerBase
    {
        public PublicController()
        {
        }

        [AllowAnonymous]
        [HttpGet("Version")]
        public object Version()
        {
            return new { Version = "1.00" };
        }

        [AllowAnonymous]
        [HttpGet("AppSettings")]
        public object GetAppSettings()
        {
            return AppSettings.Settings;
        }

        [AllowAnonymous]
        [HttpGet("TestException")]
        public void TestException()
        {
            throw new Exception("Exception occurred!");
        }

        [Authorize]
        [HttpGet("TestAuthentication")]
        public ActionResult TestAuthentication()
        {
            return Ok();
        }
    }
}
