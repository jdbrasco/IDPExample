using System;
using System.Threading;
using System.Threading.Tasks;
using IDPExample.SSO.Models;
using Microsoft.AspNetCore.Identity;

namespace IDPExample.SSO.Identity {
    public class PasswordlessUserStore :
        IUserStore<PasswordlessUser>,
        IUserPasswordStore<PasswordlessUser>,
        IUserEmailStore<PasswordlessUser>,
        IUserTwoFactorStore<PasswordlessUser>,
        IUserSecurityStampStore<PasswordlessUser>        
    {

        private readonly IServiceProvider _serviceProvider;
        public PasswordlessUserStore(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public async Task<IdentityResult> CreateAsync(PasswordlessUser user, CancellationToken cancellationToken)
        {
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(PasswordlessUser user, CancellationToken cancellationToken)
        {
             return IdentityResult.Success;
        }

        public void Dispose() { }

        public async Task<PasswordlessUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
           return new PasswordlessUser {
               Email = normalizedEmail,
               Name = "Alice"
           };
        }

        public async Task<PasswordlessUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            return new PasswordlessUser {
               Email = userId,
               Name = "Alice"
           };
        }

        public async Task<PasswordlessUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            return await FindByEmailAsync(normalizedUserName, cancellationToken);
        }

        public Task<string> GetEmailAsync(PasswordlessUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(PasswordlessUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.EmailVerified);
        }

        public Task<string> GetNormalizedEmailAsync(PasswordlessUser user, CancellationToken cancellationToken)
        {
           return Task.FromResult(user.Email);
        }

        public Task<string> GetNormalizedUserNameAsync(PasswordlessUser user, CancellationToken cancellationToken)
        {
           return Task.FromResult(user.Email);
        }

        public Task<string> GetPasswordHashAsync(PasswordlessUser user, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> GetSecurityStampAsync(PasswordlessUser user, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> GetTwoFactorEnabledAsync(PasswordlessUser user, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> GetUserIdAsync(PasswordlessUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id.ToString());
        }

        public Task<string> GetUserNameAsync(PasswordlessUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Email);
        }

        public Task<bool> HasPasswordAsync(PasswordlessUser user, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task SetEmailAsync(PasswordlessUser user, string email, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task SetEmailConfirmedAsync(PasswordlessUser user, bool confirmed, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task SetNormalizedEmailAsync(PasswordlessUser user, string normalizedEmail, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task SetNormalizedUserNameAsync(PasswordlessUser user, string normalizedName, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task SetPasswordHashAsync(PasswordlessUser user, string passwordHash, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task SetSecurityStampAsync(PasswordlessUser user, string stamp, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task SetTwoFactorEnabledAsync(PasswordlessUser user, bool enabled, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task SetUserNameAsync(PasswordlessUser user, string userName, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<IdentityResult> UpdateAsync(PasswordlessUser user, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}