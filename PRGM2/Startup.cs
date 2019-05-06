using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using IdentityModel;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;

namespace IDPExample.WEB2
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        
            services.AddAuthentication(options => {

                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = "oidc";

            }).AddCookie(options => {

                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                options.Cookie.Name = "idphybrid";

            }).AddOpenIdConnect("oidc", options => {

                    options.SignInScheme = "Cookies";
                    options.Authority = Constants.Authority;
                    options.RequireHttpsMetadata = false;

                    options.ClientSecret = "49C1A7E1-0C79-4A89-A3D6-A37998FB86B0";
                    options.ClientId = "idp_client_prog2";

                    options.ResponseType = "code id_token";

                    //options.Scope.Clear();
                    //options.Scope.Add("openid");
                    //options.Scope.Add("profile");
                    //options.Scope.Add("email");
                    options.Scope.Add("idp_client_api");
                    options.Scope.Add("offline_access");

                    //options.ClaimActions.MapAllExcept("iss", "nbf", "exp", "aud", "nonce", "iat", "c_hash");

                    options.GetClaimsFromUserInfoEndpoint = true;
                    options.SaveTokens = true;

                   // options.TokenValidationParameters = new TokenValidationParameters
                   // {
                   //     NameClaimType = JwtClaimTypes.Name,
                   //     RoleClaimType = JwtClaimTypes.Role,
                   // };


            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();

            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
