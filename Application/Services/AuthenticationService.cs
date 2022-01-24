using System;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Demo.Services
{
    public class TokenAuthenticationSchemeOptions : AuthenticationSchemeOptions
    {
    }

    public class AuthenticationService : AuthenticationHandler<TokenAuthenticationSchemeOptions>
    {
        public AuthenticationService(
            IOptionsMonitor<TokenAuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            Task<AuthenticateResult> result = Task.FromResult(AuthenticateResult.Fail("Not authorized."));

            // Authentication confirms that users are who they say they are.
            // Authorization gives those users permission to access a resource.

            if (Request.Headers.ContainsKey("yourAuthKey"))
            {
                // Verify the key...

                // If verified, optionally add some claims about the user...
                var claims = new[]
                {
                    new Claim("[key]", "value"),
                };

                // Generate claimsIdentity on the name of the class:
                var claimsIdentity = new ClaimsIdentity(claims, nameof(AuthenticationService));

                // Generate AuthenticationTicket from the Identity
                // and current authentication scheme.
                var ticket = new AuthenticationTicket(new ClaimsPrincipal(claimsIdentity), Scheme.Name);

                result = Task.FromResult(AuthenticateResult.Success(ticket));
            }

            return result;
        }
    }
}
