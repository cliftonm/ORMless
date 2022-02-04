using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Clifton;
using Interfaces;

namespace Demo.Services
{
    public class UserHasEntityPermission : IAuthorizationRequirement
    {
        public UserHasEntityPermission()
        {
        }
    }

    public class EntityAuthenticationService : AuthorizationHandler<UserHasEntityPermission>
    {
        private readonly IAccountService acctSvc;
        private readonly IEntityService entityService;

        public EntityAuthenticationService(IAccountService accountService, IEntityService entityService)
        {
            acctSvc = accountService;
            this.entityService = entityService;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, UserHasEntityPermission requirement)
        {
            var claims = context.User.Identity as ClaimsIdentity;
            var token = claims.FindFirst("token").Value;
            // var url = claims.FindFirst("url").Value;
            var method = claims.FindFirst("method").Value;
            var path = claims.FindFirst("path").Value;
            var authorized = false;

            if (path.StartsWith("/entity/"))
            {
                var entityName = path.RightOf("/entity/").LeftOf("/");
                var user = acctSvc.GetUser(token);
                authorized = user.IsSysAdmin;

                if (!authorized)
                {
                    authorized = entityService.IsUserActionAuthorized(entityName, user.Id, method);
                }
            }

            if (authorized)
            { 
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
