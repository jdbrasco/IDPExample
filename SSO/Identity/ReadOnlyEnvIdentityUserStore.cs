using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using IdentityModel;
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

        public async override Task<IList<Claim>> GetClaimsAsync(IdentityUser user, CancellationToken cancellationToken)
        {
           return new Claim[]{ 
                            new Claim(JwtClaimTypes.Name, "Alice Smith"),
                            new Claim(JwtClaimTypes.GivenName, "Alice"),
                            new Claim(JwtClaimTypes.FamilyName, "Smith"),
                            new Claim(JwtClaimTypes.Email, "alicesmith@email.com"),
                            new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                            new Claim(JwtClaimTypes.WebSite, "http://alice.com"),
                            new Claim(JwtClaimTypes.Address, @"{ 'street_address': 'One Hacker Way', 'locality': 'Heidelberg', 'postal_code': 69118, 'country': 'Germany' }", IdentityServer4.IdentityServerConstants.ClaimValueTypes.Json)
                        };

        }
    }

}