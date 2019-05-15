using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;

namespace IDPExample.SSO.Identity {

    public class PasswordlessProfileService : IProfileService
    {
        private readonly UserManager<IdentityUser> _userManager; 

        public PasswordlessProfileService(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var sub = context.Subject.FindFirst("sub")?.Value;
            if (sub != null) {

                var user = await _userManager.FindByNameAsync(sub);
                var cp = await getClaims(user);
 
                var claims = cp.Claims;
                if (context.RequestedClaimTypes != null && context.RequestedClaimTypes.Any())
                {
                    claims = claims.Where(x => context.RequestedClaimTypes.Contains(x.Type)).ToArray().AsEnumerable();
                }
 
                context.IssuedClaims = claims.ToList();

            }
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {   
            var securityTokenClaim = context.Subject?.Claims.FirstOrDefault(c => c.Type == "sstamp");
            var sub = context.Subject.FindFirst("sub")?.Value;

            //var user = await _userManager.GetUserByPrincipalAsync(context.Subject);
            var user = await _userManager.FindByNameAsync(sub);

            if(user != null && securityTokenClaim != null)
            {
                context.IsActive = string.Equals(user.SecurityStamp, securityTokenClaim.Value,
                    StringComparison.InvariantCultureIgnoreCase);
                return;
            }
            else
            {
                context.IsActive = true;
            }
        }

        private async Task<ClaimsPrincipal> getClaims(IdentityUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
                         
            var id = new ClaimsIdentity();            
            id.AddClaim(new Claim(JwtClaimTypes.PreferredUserName, user.UserName));
 
            id.AddClaims(await _userManager.GetClaimsAsync(user));
 
            return new ClaimsPrincipal(id);
        }
    }

}