﻿using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace northopsService.Models
{
    public partial class UserStore :
        IQueryableUserStore<User>, IUserPasswordStore<User>, IUserLoginStore<User, string>,
        IUserClaimStore<User, string>, IUserRoleStore<User, string>, IUserSecurityStampStore<User, string>,
        IUserEmailStore<User, string>, IUserPhoneNumberStore<User, string>, IUserTwoFactorStore<User, string>,
        IUserLockoutStore<User, string>
    {
        private readonly NorthopsDbEntities db;

        public UserStore(NorthopsDbEntities db)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }

            this.db = db;
        }

        //// IQueryableUserStore<User, int>

        public IQueryable<User> Users
        {
            get { return this.db.Users; }
        }

        //// IUserStore<User, Key>

        public Task CreateAsync(User user)
        {
            this.db.Users.Add(user);
            return this.db.SaveChangesAsync();
        }

        public Task DeleteAsync(User user)
        {
            this.db.Users.Remove(user);
            return this.db.SaveChangesAsync();
        }

        public Task<User> FindByIdAsync(string userId)
        {
            return this.db.Users
                .Include(u => u.UserLogins).Include(u => u.UserRoles).Include(u => u.UserClaims)
                .FirstOrDefaultAsync(u => u.Id.Equals(userId));
        }

        public Task<User> FindByNameAsync(string userName)
        {
            return this.db.Users
                .Include(u => u.UserLogins).Include(u => u.UserRoles).Include(u => u.UserClaims)
                .FirstOrDefaultAsync(u => u.UserName == userName);
        }

        public Task UpdateAsync(User user)
        {
            this.db.Entry<User>(user).State = EntityState.Modified;
            return this.db.SaveChangesAsync();
        }

        //// IUserPasswordStore<User, Key>

        public Task<string> GetPasswordHashAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(User user)
        {
            return Task.FromResult(user.PasswordHash != null);
        }

        public Task SetPasswordHashAsync(User user, string passwordHash)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            user.PasswordHash = passwordHash;
            return Task.FromResult(0);
        }

        //// IUserLoginStore<User, Key>

        public Task AddLoginAsync(User user, UserLoginInfo login)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (login == null)
            {
                throw new ArgumentNullException("login");
            }

            var userLogin = Activator.CreateInstance<UserLogin>();
            userLogin.UserId = user.Id;
            userLogin.LoginProvider = login.LoginProvider;
            userLogin.ProviderKey = login.ProviderKey;
            user.UserLogins.Add(userLogin);
            return Task.FromResult(0);
        }

        public async Task<User> FindAsync(UserLoginInfo login)
        {
            if (login == null)
            {
                throw new ArgumentNullException("login");
            }

            var provider = login.LoginProvider;
            var key = login.ProviderKey;

            var userLogin = await this.db.UserLogins.FirstOrDefaultAsync(l => l.LoginProvider == provider && l.ProviderKey == key);

            if (userLogin == null)
            {
                return default(User);
            }

            return await this.db.Users
                .Include(u => u.UserLogins).Include(u => u.UserRoles).Include(u => u.UserClaims)
                .FirstOrDefaultAsync(u => u.Id.Equals(userLogin.UserId));
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return Task.FromResult<IList<UserLoginInfo>>(user.UserLogins.Select(l => new UserLoginInfo(l.LoginProvider, l.ProviderKey)).ToList());
        }

        public Task RemoveLoginAsync(User user, UserLoginInfo login)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (login == null)
            {
                throw new ArgumentNullException("login");
            }

            var provider = login.LoginProvider;
            var key = login.ProviderKey;

            var item = user.UserLogins.SingleOrDefault(l => l.LoginProvider == provider && l.ProviderKey == key);

            if (item != null)
            {
                user.UserLogins.Remove(item);
            }

            return Task.FromResult(0);
        }

        //// IUserClaimStore<User, int>

        public Task AddClaimAsync(User user, Claim claim)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (claim == null)
            {
                throw new ArgumentNullException("claim");
            }

            var item = Activator.CreateInstance<UserClaim>();
            item.UserId = user.Id;
            item.ClaimType = claim.Type;
            item.ClaimValue = claim.Value;
            user.UserClaims.Add(item);
            return Task.FromResult(0);
        }

        public Task<IList<Claim>> GetClaimsAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return Task.FromResult<IList<Claim>>(user.UserClaims.Select(c => new Claim(c.ClaimType, c.ClaimValue)).ToList());
        }

        public Task RemoveClaimAsync(User user, Claim claim)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (claim == null)
            {
                throw new ArgumentNullException("claim");
            }

            foreach (var item in user.UserClaims.Where(uc => uc.ClaimValue == claim.Value && uc.ClaimType == claim.Type).ToList())
            {
                user.UserClaims.Remove(item);
            }

            foreach (var item in this.db.UserClaims.Where(uc => uc.UserId.Equals(user.Id) && uc.ClaimValue == claim.Value && uc.ClaimType == claim.Type).ToList())
            {
                this.db.UserClaims.Remove(item);
            }

            return Task.FromResult(0);
        }

        //// IUserRoleStore<User, int>

        public Task AddToRoleAsync(User user, string roleName)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (string.IsNullOrWhiteSpace(roleName))
            {
                throw new ArgumentException("Value cannot be null or empty.", "roleName");
            }

            var userRole = this.db.UserRoles.SingleOrDefault(r => r.Name == roleName);

            if (userRole == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Role {0} does not exist.", new object[] { roleName }));
            }

            user.UserRoles.Add(userRole);
            return Task.FromResult(0);
        }

        public Task<IList<string>> GetRolesAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return Task.FromResult<IList<string>>(user.UserRoles.Join(this.db.UserRoles, ur => ur.Id, r => r.Id, (ur, r) => r.Name).ToList());
        }

        public Task<bool> IsInRoleAsync(User user, string roleName)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (string.IsNullOrWhiteSpace(roleName))
            {
                throw new ArgumentException("Value cannot be null or empty.", "roleName");
            }

            return
                Task.FromResult<bool>(
                    this.db.UserRoles.Any(r => r.Name == roleName && r.Users.Any(u => u.Id.Equals(user.Id))));
        }

        public Task RemoveFromRoleAsync(User user, string roleName)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (string.IsNullOrWhiteSpace(roleName))
            {
                throw new ArgumentException("Value cannot be null or empty.", "roleName");
            }

            var userRole = user.UserRoles.SingleOrDefault(r => r.Name == roleName);

            if (userRole != null)
            {
                user.UserRoles.Remove(userRole);
            }

            return Task.FromResult(0);
        }

        //// IUserSecurityStampStore<User, int>

        public Task<string> GetSecurityStampAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return Task.FromResult(user.SecurityStamp);
        }

        public Task SetSecurityStampAsync(User user, string stamp)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            user.SecurityStamp = stamp;
            return Task.FromResult(0);
        }

        //// IUserEmailStore<User, int>

        public Task<User> FindByEmailAsync(string email)
        {
            return this.db.Users
                .Include(u => u.UserLogins).Include(u => u.UserRoles).Include(u => u.UserClaims)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public Task<string> GetEmailAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return Task.FromResult(user.EmailConfirmed);
        }

        public Task SetEmailAsync(User user, string email)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            user.Email = email;
            return Task.FromResult(0);
        }

        public Task SetEmailConfirmedAsync(User user, bool confirmed)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            user.EmailConfirmed = confirmed;
            return Task.FromResult(0);
        }

        //// IUserPhoneNumberStore<User, int>

        public Task<string> GetPhoneNumberAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return Task.FromResult(user.PhoneNumber);
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return Task.FromResult(user.PhoneNumberConfirmed);
        }

        public Task SetPhoneNumberAsync(User user, string phoneNumber)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            user.PhoneNumber = phoneNumber;
            return Task.FromResult(0);
        }

        public Task SetPhoneNumberConfirmedAsync(User user, bool confirmed)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            user.PhoneNumberConfirmed = confirmed;
            return Task.FromResult(0);
        }

        //// IUserTwoFactorStore<User, int>

        public Task<bool> GetTwoFactorEnabledAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return Task.FromResult(user.TwoFactorEnabled);
        }

        public Task SetTwoFactorEnabledAsync(User user, bool enabled)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            user.TwoFactorEnabled = enabled;
            return Task.FromResult(0);
        }

        //// IUserLockoutStore<User, int>

        public Task<int> GetAccessFailedCountAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return Task.FromResult(user.AccessFailedCount);
        }

        public Task<bool> GetLockoutEnabledAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return Task.FromResult(user.LockoutEnabled);
        }

        public Task<DateTimeOffset> GetLockoutEndDateAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return Task.FromResult(
                user.LockoutEndDateUtc.HasValue ?
                    new DateTimeOffset(DateTime.SpecifyKind(user.LockoutEndDateUtc.Value, DateTimeKind.Utc)) :
                    new DateTimeOffset());
        }

        public Task<int> IncrementAccessFailedCountAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            user.AccessFailedCount++;
            return Task.FromResult(user.AccessFailedCount);
        }

        public Task ResetAccessFailedCountAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            user.AccessFailedCount = 0;
            return Task.FromResult(0);
        }

        public Task SetLockoutEnabledAsync(User user, bool enabled)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            user.LockoutEnabled = enabled;
            return Task.FromResult(0);
        }

        public Task SetLockoutEndDateAsync(User user, DateTimeOffset lockoutEnd)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            user.LockoutEndDateUtc = lockoutEnd == DateTimeOffset.MinValue ? null : new DateTime?(lockoutEnd.UtcDateTime);
            return Task.FromResult(0);
        }

        //// IDisposable

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && this.db != null)
            {
                this.db.Dispose();
            }
        }


    }
}