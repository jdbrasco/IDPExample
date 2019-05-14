using System;
using System.Threading;
using System.Threading.Tasks;
using IDPExample.SSO.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace IDPExample.SSO.Identity {

    public class ReadOnlyEnvIdentityUserStore : ReadOnlyIdentityUserStore
    {
        private readonly IConfiguration _configuration;

        public ReadOnlyEnvIdentityUserStore(IConfiguration configuration)
        {
            _configuration = configuration;
            
        }
        public override Task<IdentityUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(new IdentityUser {

                Id = normalizedEmail,
                Email = normalizedEmail,
                NormalizedEmail = normalizedEmail,
                EmailConfirmed = true,
                UserName = normalizedEmail,
                NormalizedUserName = normalizedEmail,
                SecurityStamp = Guid.NewGuid().ToString("D")
            });
        }


        public override Task<IdentityUser> FindByIdAsync(string userId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return FindByEmailAsync(userId, cancellationToken);
        }
    }

}