using System;
using System.Linq;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Clifton;
using Interfaces;
using Lib;
using Models.Requests;
using Models.Responses;

namespace Clifton.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : PluginController
    {
        public override string Version => "1.00";

        private readonly IAccountService svc;

        public AccountController(IAccountService svc)
        {
            this.svc = svc;
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public ActionResult Login(LoginRequest req)
        {
            var resp = svc.Login(req);

            var ret = resp == null ? (ActionResult)Unauthorized("User not found.") : Ok(resp);

            return ret;
        }

        [Authorize]
        [HttpPost()]
        public ActionResult CreateAccount(LoginRequest req)
        {
            ActionResult ret = Ok();

            if (!svc.CreateAccount(req))
            { 
                ret = BadRequest($"Username {req.Username} already exists.");
            }

            return ret;
        }
    }
}
