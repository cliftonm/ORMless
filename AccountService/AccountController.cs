﻿using System;
using System.Linq;
using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Clifton;
using Interfaces;
using Lib;
using Models.Requests;
using Models.Responses;

namespace Clifton.Controllers
{
    // Sort of an implementation of RFC 6750 https://datatracker.ietf.org/doc/html/rfc6750 or https://www.rfc-editor.org/rfc/rfc6750.txt, 
    // but doesn't require form encoding, which I basically despise anyways.

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
        public ActionResult Login(AccountRequest req)
        {
            var resp = svc.Login(req);

            var ret = resp == null ? (ActionResult)Unauthorized("User not found.") : Ok(resp);

            return ret;
        }

        [AllowAnonymous]
        [HttpPost("Refresh")]
        public ActionResult Refresh(AccountRequest req)
        {
            var resp = svc.Login(req);

            var ret = resp == null ? (ActionResult)Unauthorized("User not found.") : Ok(resp);

            return ret;
        }

        [Authorize]
        [HttpPost("Logout")]
        public ActionResult Logout()
        {
            var claims = User.Identity as ClaimsIdentity;
            var token = claims.FindFirst("token").Value;

            svc.Logout(token);

            return Ok();
        }

        [Authorize]
        [HttpPost()]
        public ActionResult CreateAccount(AccountRequest req)
        {
            ActionResult ret = Ok();

            if (!svc.CreateAccount(req))
            { 
                ret = BadRequest($"Username {req.Username} already exists.");
            }

            return ret;
        }

        /// <summary>
        /// A user can only delete their own account.  
        /// </summary>
        [Authorize]
        [HttpDelete()]
        public ActionResult DeleteAccount()
        {
            ActionResult ret = Ok();

            var claims = User.Identity as ClaimsIdentity;
            var token = claims.FindFirst("token").Value;
            svc.DeleteAccount(token);

            return ret;
        }

        [Authorize]
        [HttpPatch()]
        public ActionResult ChangeUsernameAndPassword(AccountRequest req)
        {
            ActionResult ret = Ok();

            return ret;
        }
    }
}
