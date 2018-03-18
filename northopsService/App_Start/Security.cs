using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using northopsService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace northopsService.App_Start
{
    public class ApplicationUserManager : UserManager<User, string>
    {
        public ApplicationUserManager(IUserStore<User, string> store) : base(store)
        {
        }
        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var userStore = new UserStore(context.Get<NorthopsDbEntities>());
            var userManager = new UserManager<User, string>(userStore);

            var manager = new ApplicationUserManager(userStore);

            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<User>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false,
            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
         /*   manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<User>
            {
                MessageFormat = "Your security code is {0}"
            });
            manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<User>
            {
                Subject = "Security Code",
                BodyFormat = "Your security code is {0}"
            });*/
            //manager.EmailService = new EmailService();
            //manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<User>(dataProtectionProvider.Create("ASP.NET Identity"));
            }

            return manager;
        }
        public async Task<IdentityResult> ChangePasswordAsync(User userId, string newPassword)
        {
            var store = this.Store as IUserPasswordStore<User, string>;
            //if (store == null) {
            //    var errors = new string[]
            //    {
            //    "Current UserStore doesn't implement IUserPasswordStore"
            //    };

            //    return Task.FromResult<IdentityResult>(new IdentityResult(errors) {  Succeeded = false });
            //}

            var newPasswordHash = this.PasswordHasher.HashPassword(newPassword);

            await store.SetPasswordHashAsync(userId, newPasswordHash);
            return await Task.FromResult<IdentityResult>(IdentityResult.Success);
        }

    }
    public class ApplicationSignInManager : SignInManager<User, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }
        public override Task<ClaimsIdentity> CreateUserIdentityAsync(User user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }
        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }
}