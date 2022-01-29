using System;
using System.Linq;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Clifton;
using Interfaces;
using Lib;
using Models;

using Clifton.Requests;

namespace Clifton.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : PluginController
    {
        public override string Version => "1.00";

        private readonly IAppDbContext context;

        public AccountController(IAppDbContext context)
        {
            this.context = context; 
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public ActionResult Login(LoginRequest req)
        {
            return Ok();
        }

        [Authorize]
        public ActionResult CreateAccount(LoginRequest req)
        {
            ActionResult ret;

            var existingUsers = context.User.Where(u => u.UserName == req.Username && !u.Deleted).Count();

            if (existingUsers == 0)
            {
                var salt = Hasher.GenerateSalt();
                var hashedPassword = Hasher.HashPassword(salt, req.Password);
                var user = new User() { UserName = req.Username, Password = hashedPassword, Salt = salt };
                context.User.Add(user);
                context.SaveChanges();
                ret = Ok();
            }
            else
            {
                ret = BadRequest($"Username {req.Username} already exists.");
            }

            return ret;
        }
    }
}
