using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Clifton;
using Interfaces;
using Lib;

namespace Demo.Services
{
    public class TokenAuthenticationSchemeOptions : AuthenticationSchemeOptions
    {
    }

    public class TokenAuthenticationService : AuthenticationHandler<TokenAuthenticationSchemeOptions>
    {
        private readonly IAccountService acctSvc;

        public TokenAuthenticationService(
            IAppDbContext context,
            IAccountService accountService,
            IOptionsMonitor<TokenAuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock) : base(options, logger, encoder, clock)
        {
            acctSvc = accountService;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            Task<AuthenticateResult> result = Task.FromResult(AuthenticateResult.Fail("Not authorized."));

            // Authentication confirms that users are who they say they are.
            // Authorization gives those users permission to access a resource.

            if (Request.Headers.ContainsKey(Constants.AUTHORIZATION))
            {
                var token = Request.Headers[Constants.AUTHORIZATION][0].RightOf(Constants.TOKEN_PREFIX).Trim();
                bool verified = acctSvc.VerifyAccount(token);

                if (verified)
                {
                    var request = Request.HttpContext.Request;
                    var basePath = $"{request.Scheme}://{request.Host}{request.PathBase}{request.Path}";

                    // If verified, optionally add some claims about the user...
                    var claims = new[]
                    {
                        new Claim("token", token),
                        new Claim("url", basePath),
                        new Claim("method", request.Method),
                        new Claim("path", request.Path)
                    };

                    // Generate claimsIdentity on the name of the class:
                    var claimsIdentity = new ClaimsIdentity(claims, nameof(TokenAuthenticationService));

                    // Generate AuthenticationTicket from the Identity
                    // and current authentication scheme.
                    var ticket = new AuthenticationTicket(new ClaimsPrincipal(claimsIdentity), Scheme.Name);

                    result = Task.FromResult(AuthenticateResult.Success(ticket));
                }
            }

            return result;
        }
    }
}
