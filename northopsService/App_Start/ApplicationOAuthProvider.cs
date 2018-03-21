using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Azure.Mobile.Server.Login;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using northopsService.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Security.Claims;
using System.Threading.Tasks;

namespace northopsService.App_Start
{
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
        private readonly string _publicClientId;
        private ApplicationUserManager _userManager;

        public ApplicationOAuthProvider(string publicClientId)
        {
            if (publicClientId == null)
            {
                throw new ArgumentNullException("publicClientId");
            }

            _publicClientId = publicClientId;
        }
        public ApplicationUserManager UserManager
        {
            get { return _userManager; }
            private set
            {
                _userManager = value;
            }
        }
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();

            User user = await userManager.FindByEmailAsync(context.UserName) ?? await userManager.FindByNameAsync(context.UserName);
            if (user != null)
                user = await userManager.FindAsync(user.UserName, context.Password);

            if (user == null)
            {
                context.SetError("invalid_grant", "The user name or password is incorrect.");
                return;
            }

            ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(userManager,
               OAuthDefaults.AuthenticationType);
            ClaimsIdentity cookiesIdentity = await user.GenerateUserIdentityAsync(userManager,
                CookieAuthenticationDefaults.AuthenticationType);



            AuthenticationProperties properties = CreateProperties(user.UserName);
            AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity, properties);
            context.Validated(ticket);
            context.Request.Context.Authentication.SignIn(cookiesIdentity);
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            // Resource owner password credentials does not provide a client ID.
            if (context.ClientId == null)
            {
                context.Validated();
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            if (context.ClientId == _publicClientId)
            {
                Uri expectedRootUri = new Uri(context.Request.Uri, "/");

                if (expectedRootUri.AbsoluteUri == context.RedirectUri)
                {
                    context.Validated();
                }
            }

            return Task.FromResult<object>(null);
        }

        public static AuthenticationProperties CreateProperties(string userName)
        {
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "userName", userName },
               { "JWToken",GenerateJWToken(userName)}
            };
            return new AuthenticationProperties(data);
        }
        public static string GenerateJWToken(string UserName)
        {
            var db = new NorthopsContext ();
            var signingKey = Environment.GetEnvironmentVariable("WEBSITE_AUTH_SIGNING_KEY");

            //Environment.GetEnvironmentVariable("WEBSITE_AUTH_SIGNING_KEY");
            var website = Environment.GetEnvironmentVariable("WEBSITE_HOSTNAME");
            var audience = $"https://{website}/";
            var issuer = $"https://{website}/";
#if DEBUG
            signingKey = "E2EED04CCCED91FD8170172FD529DAB9E32D65CEF7A50F9FE8545921135A77B4";
            audience = "http://localhost:50782/";
            issuer = "http://localhost:50782/";
#endif
            var claims = new Claim[] { new Claim(JwtRegisteredClaimNames.Sub, UserName) };
            JwtSecurityToken token = AppServiceLoginHandler.CreateToken(
             claims, signingKey, audience, issuer, TimeSpan.FromDays(30));
            return token.RawData;
        }
    }

}