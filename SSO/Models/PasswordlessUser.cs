using System;
using Microsoft.AspNetCore.Identity;

namespace IDPExample.SSO.Models {

    public class PasswordlessUser
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool EmailVerified { get; set; }
        public string SecurityStamp { get; set; }

        public Guid? GetUserId()
        {
            return Id;
        }

        public IdentityUser ToIdentityUser(bool twoFactorEnabled)
        {
            return new IdentityUser
            {
                Id = Id.ToString(),
                Email = Email,
                NormalizedEmail = Email,
                EmailConfirmed = EmailVerified,
                UserName = Email,
                NormalizedUserName = Email,
                TwoFactorEnabled = twoFactorEnabled,
                SecurityStamp = SecurityStamp
            };
        }
    }
}