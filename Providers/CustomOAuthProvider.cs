using $safeprojectname$.Infrastructure;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace $safeprojectname$.Providers
{
    public class CustomOAuthProvider : OAuthAuthorizationServerProvider
    {
        // This method is "empty", we are considering the request valid always.
        // Because we assume the client is a trusted one we don't need to validate it. 
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
            return Task.FromResult<object>(null);
        }

        // Recieves username and password from request via context
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var allowedOrigin = "*";

            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });

            // Get userManager
            var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();

            // Get user, using Presented UserName and Password
            ApplicationUser user = await userManager.FindAsync(context.UserName, context.Password);

            // User not found?
            if (user == null)
            {
                context.SetError("invalid_grant", "The user name or password is incorrect.");
                return;
            }

            // Has the user not confirmed Email? - I have not implemented Email service yet, so I comment out.
            //if (!user.EmailConfirmed)
            //{
            //    context.SetError("invalid_grant", "User did not confirm email");
            //    return;
            //}

            // Method: "GenerateUserIdentityAsync" lives in the ApplicationUser class
            // Build identity for the logged in user, the identity will contain all the roles and claims for the authenticated user. 
            ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(userManager, "JWT");

            // Create Authentication ticket containing the identity of the user.
            var ticket = new AuthenticationTicket(oAuthIdentity, null);

            // Transfer the identity to an OAuth 2.0 bearer access token.
            context.Validated(ticket);
        }
    }
}
