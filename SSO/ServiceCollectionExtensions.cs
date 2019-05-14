using System;
using IDPExample.SSO.Data;
using IDPExample.SSO.Identity;
using IDPExample.SSO.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace IDPExample.SSO
{
    public static class ServiceCollectionExtensions {

        public static void AddCustomDataProtectionServices(this IServiceCollection services) {

            
            return;
        }

        public static Tuple<IdentityBuilder, IdentityBuilder> AddPasswordlessIdentityServices<TUserStore>(
            this IServiceCollection services, IConfiguration configuration) where TUserStore : class {
            
            services.AddScoped<DbContext, ApplicationDbContext>();

            services.Configure<DataProtectionTokenProviderOptions>(options => {

                options.TokenLifespan = TimeSpan.FromMinutes(15);
            });

            services.TryAddScoped<PasswordlessSignInManager<IdentityUser>, PasswordlessSignInManager<IdentityUser>>();

            var passwordlessIdentityBuilder = services.AddIdentity<IdentityUser, Role>()
                .AddSignInManager<PasswordlessSignInManager<IdentityUser>>()
                .AddUserStore<TUserStore>()
                .AddRoleStore<RoleStore>()
                .AddUserManager<UserManager<IdentityUser>>()
                //.AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
                
                
            // var regularIdentityBuilder = services.AddIdentity<ApplicationUser, IdentityRole>()
            //     .AddEntityFrameworkStores<ApplicationDbContext>()
            //     .AddDefaultTokenProviders();

            var regularIdentityBuilder = services.AddIdentityCore<ApplicationUser>()
                    .AddEntityFrameworkStores<ApplicationDbContext>()
                    .AddSignInManager()
                    //.AddUserStore<UserStore>()
                    //.AddRoleStore<Microsoft.AspNetCore.Identity.EntityFrameworkCore.RoleStore<Microsoft.AspNetCore.Identity.IdentityRole>>()
                    //.AddRoles<Microsoft.AspNetCore.Identity.IdentityRole>()
                    .AddUserManager<UserManager<ApplicationUser>>();
                    //.AddDefaultTokenProviders();
             //var regularIdentityBuilder = services.AddIdentityCore<PasswordlessUser>()
            //     .AddEntityFrameworkStores<ApplicationDbContext>()
            //     .AddSignInManager()
            //     .AddDefaultTokenProviders();;

                //.AddUserStore<PasswordlessUserStore>();


            services.ConfigureApplicationCookie(options => {
                options.Cookie.Name = "idp_passwordless";
                 options.LoginPath = "/Legacy";
                 options.LogoutPath = "/";
                 options.AccessDeniedPath = "/Legacy?accessDenied=true";
                 options.Cookie.Name = "IdpExample_PRGM2";
                 options.Cookie.Expiration = options.ExpireTimeSpan = TimeSpan.FromDays(2);
                 options.ReturnUrlParameter = "returnUrl";
                 options.SlidingExpiration = true;
             });
            
           return new Tuple<IdentityBuilder, IdentityBuilder>(passwordlessIdentityBuilder, regularIdentityBuilder);
        }
    }
}