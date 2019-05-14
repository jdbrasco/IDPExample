using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IDPExample.SSO.Identity {
    public class PasswordlessSignInManager<TUser> : SignInManager<TUser> where TUser : class
    {
        public const string PasswordlessSignInPurpose = "PasswordlessSignIn";

        public PasswordlessSignInManager(
            UserManager<TUser> userManager, 
            IHttpContextAccessor contextAccessor, 
            IUserClaimsPrincipalFactory<TUser> claimsFactory, 
            IOptions<IdentityOptions> optionsAccessor, 
            ILogger<SignInManager<TUser>> logger, 
            IAuthenticationSchemeProvider schemes) 
                : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes)
        {

            
        }

        public async Task<string> PasswordlessSignInAsync(string email, string returnUrl)
        {
            var user = await UserManager.FindByEmailAsync(email);
            if(user == null)
            {
                return "failed";
            }

            // var token = await UserManager.GenerateUserTokenAsync(user, Options.Tokens.PasswordResetTokenProvider,
            //     PasswordlessSignInPurpose);

            var token = await UserManager.GenerateUserTokenAsync(user, "Default", PasswordlessSignInPurpose);
            //var verify = await UserManager.VerifyUserTokenAsync(user, "Default", PasswordlessSignInPurpose, token);

            var url = ExtendQuery(new Uri($"https://localhost:5191/Legacy/Confirm"),
            new Dictionary<string, string>
            {
                ["returnUrl"] = returnUrl,
                ["email"] = email,
                ["token"] = token,
            });

            return url.ToString();;
        }

        public async Task<SignInResult> PasswordlessSignInAsync(TUser user, string token, bool isPersistent)
        {
            if(user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

           // var verify = await UserManager.VerifyUserTokenAsync(user, "Default", PasswordlessSignInPurpose, token);

            var attempt = await CheckPasswordlessSignInAsync(user, token);
            return attempt.Succeeded ?
                await SignInOrTwoFactorAsync(user, isPersistent, bypassTwoFactor: true) : attempt;
        }

        public async Task<SignInResult> PasswordlessSignInAsync(string email, string token, bool isPersistent)
        {
            var user = await UserManager.FindByEmailAsync(email);
            if(user == null)
            {
                return SignInResult.Failed;
            }

            return await PasswordlessSignInAsync(user, token, isPersistent);
        }

        public virtual async Task<SignInResult> CheckPasswordlessSignInAsync(TUser user, string token)
        {
            if(user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var error = await PreSignInCheck(user);
            if(error != null)
            {
                return error;
            }

            var verify = await UserManager.VerifyUserTokenAsync(user, "Default", PasswordlessSignInPurpose, token);
            
            if(verify)
            {
                return SignInResult.Success;
            }

            return SignInResult.Failed;
        }

        private static Uri ExtendQuery(Uri uri, IDictionary<string, string> values)
        {
            var baseUri = uri.ToString();
            var queryString = string.Empty;
            if(baseUri.Contains("?"))
            {
                var urlSplit = baseUri.Split('?');
                baseUri = urlSplit[0];
                queryString = urlSplit.Length > 1 ? urlSplit[1] : string.Empty;
            }

            var queryCollection = HttpUtility.ParseQueryString(queryString);
            foreach(var kvp in values ?? new Dictionary<string, string>())
            {
                queryCollection[kvp.Key] = kvp.Value;
            }

            var uriKind = uri.IsAbsoluteUri ? UriKind.Absolute : UriKind.Relative;
            if(queryCollection.Count == 0)
            {
                return new Uri(baseUri, uriKind);
            }
            return new Uri(string.Format("{0}?{1}", baseUri, queryCollection), uriKind);
        }

    }
}