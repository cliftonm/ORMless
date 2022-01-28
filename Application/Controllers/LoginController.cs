using System;
using System.Linq;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Clifton;
using Interfaces;
using Models;

using Demo.Requests;

namespace Demo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly IAppDbContext context;

        public LoginController(IAppDbContext context)
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
            ActionResult ret = null;

            var existingUsers = context.User.Where(u => u.UserName == req.Username && !u.Deleted).Count();

            if (existingUsers == 0)
            {
                var salt = Hasher.GenerateSalt();
                var hashedPassword = Hasher.HashPassword(salt, req.Password);
                var user = new User() { UserName = req.Username, Password = hashedPassword, Salt = salt };
                context.User.Add(user);
                context.SaveChanges();
            }
            else
            {
                ret = BadRequest($"Username {req.Username} already exists.");
            }

            return ret;
        }
    }
}
