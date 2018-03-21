using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Azure.Mobile.Server.Config;
using Microsoft.Azure.Mobile.Server.Login;
using Newtonsoft.Json;
using northopsService.App_Start;
using northopsService.Models;

namespace northopsService.Controllers
{
    [Route(".auth/login/token")]
    public class AuthenticationController : ApiController
    {
        private northopsService.Models.NorthopsContext db;
        private string signingKey, audience, issuer;
        private string website;

        public AuthenticationController()
        {
            db = new NorthopsContext();
            signingKey = Environment.GetEnvironmentVariable("WEBSITE_AUTH_SIGNING_KEY");
            website = Environment.GetEnvironmentVariable("WEBSITE_HOSTNAME");

            audience = $"https://{website}/";
            issuer = $"https://{website}/";
#if DEBUG
            signingKey = "E2EED04CCCED91FD8170172FD529DAB9E32D65CEF7A50F9FE8545921135A77B4";

            audience = "http://localhost:50782/";
            issuer = "http://localhost:50782/";
#endif
        }

        // GET 
        [HttpPost]
        public async Task<IHttpActionResult> PostAsync(Users user)
        {

            if (!ModelState.IsValid)
                return BadRequest("Model is not valid");

            var users = await IsValidUser(user);
            if (users == null)
                return BadRequest("The user name or password is incorrect");

            var claims = new Claim[] { new Claim(JwtRegisteredClaimNames.Sub, user.Username) };
            JwtSecurityToken token = AppServiceLoginHandler.CreateToken(
             claims, signingKey, audience, issuer, TimeSpan.FromDays(30));

            return Ok(new LoginResult() { AuthenticationToken = token.RawData, User = new LoginResultUser() { UserId = users.Id } });
        }

        private async Task<User> IsValidUser(Users user)
        {
            ApplicationUserManager userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var res = await userManager.FindAsync(user.Username, user.Password);
            return res;
        }

    }

    public class LoginResult
    {
        public LoginResult()
        {
        }
        [JsonProperty(PropertyName = "authenticationToken")]
        public string AuthenticationToken { get; set; }
        [JsonProperty(PropertyName = "user")]
        public LoginResultUser User { get; set; }
    }

    public class LoginResultUser
    {
        [JsonProperty(PropertyName = "userId")]
        public object UserId { get; set; }


    }

    public class Users
    {
        public string Password { get; set; }
        public string Username { get; set; }
    }
    public class TokenResult
    {
        public string access_token { get; set; }

    }
}
