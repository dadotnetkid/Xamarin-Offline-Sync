using Microsoft.AspNet.Identity;
using System.Security.Claims;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
namespace northopsService.Models
{
    public partial class User : IUser<string>
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User, string> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);

            // Add custom user claims here
            return userIdentity;
        }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(
            UserManager<User, string> manager, string authenticationType)
        {
            // Note the authenticationType must match the one 
            // defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity =
                await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }

        [NotMapped]
        public string FullName
        {
            get
            {
                string dspFirstName = string.IsNullOrWhiteSpace(this.FirstName) ? "" : this.FirstName;
                string dspLastName = string.IsNullOrWhiteSpace(this.LastName) ? "" : this.LastName;

                return dspFirstName + " " + dspLastName;
            }
        }
        [NotMapped]
        public string Password { get; set; }
    }
}