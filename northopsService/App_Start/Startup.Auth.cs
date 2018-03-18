using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using northopsService.App_Start;
using northopsService.Models;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace northopsService
{
	public partial class Startup
	{
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

        public static string PublicClientId { get; private set; }

        // For more information on configuring authentication, please visit https://go.microsoft.com/fwlink/?LinkId=301864
        public static void ConfigureAuthWebApi(IAppBuilder app)
        {
            app.CreatePerOwinContext(NorthopsDbEntities.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);
            //app.CreatePerOwinContext<ApplicationRoleManager>(ApplicationRoleManager.Create);
          //  app.UseCookieAuthentication(new CookieAuthenticationOptions());
           // app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            /*PublicClientId = "self";
            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/Token"),
                Provider = new ApplicationOAuthProvider(PublicClientId),
                AuthorizeEndpointPath = new PathString("/api/Account/ExternalLogin"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(14),
                // In production mode set AllowInsecureHttp = false
                AllowInsecureHttp = true,
            };
            app.UseOAuthBearerTokens(OAuthOptions);*/

        }
    }
}