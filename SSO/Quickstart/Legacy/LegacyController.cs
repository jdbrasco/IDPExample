using System;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Events;
using IdentityServer4.Models;
using IdentityServer4.Quickstart.UI;
using IdentityServer4.Services;
using IDPExample.SSO.Identity;
using IDPExample.SSO.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IDPExample.SSO.Quickstart.Legacy
{
    [SecurityHeaders]
    [AllowAnonymous]
    public class LegacyController : Controller
    {
        private readonly PasswordlessSignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IAuthenticationSchemeProvider _schemeProvider;
        private readonly IEventService _events;


        public LegacyController(
            PasswordlessSignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            IIdentityServerInteractionService interaction,
            IAuthenticationSchemeProvider schemeProvider,
            IEventService events
        ) {
            
            _signInManager = signInManager;
            _userManager = userManager;
            _interaction = interaction;
            _schemeProvider = schemeProvider;
            _events = events;
        }

        [HttpGet]
        public IActionResult Index(string returnUrl = null, string error = null, string success = null, string link = null, bool accessDenied = false)
        {


            if (string.IsNullOrWhiteSpace(error) && accessDenied)
            {
                error = "Access denied. Please log in.";
            }

            return View(new PasswordlessLoginModel
            {

                ReturnUrl = returnUrl,
                Error = error,
                Success = success,
                Link = link
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(PasswordlessLoginModel model, string button){

                var context = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);

                if (ModelState.IsValid) {

                    var result = await _signInManager.PasswordlessSignInAsync(model.Email, model.ReturnUrl);

                    if (result.Succeeded) {

                        var user = await _userManager.FindByNameAsync(model.Email);                        
                        // var claims = _userManager.AddClaimsAsync(user, new Claim[]{
                        //                                 new Claim(JwtClaimTypes.Name, "Alice Smith"),
                        //                                 new Claim(JwtClaimTypes.GivenName, "Alice"),
                        //                                 new Claim(JwtClaimTypes.FamilyName, "Smith"),
                        //                                 new Claim(JwtClaimTypes.Email, "alicesmith@email.com"),
                        //                                 new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                        //                                 new Claim(JwtClaimTypes.WebSite, "http://alice.com"),
                        //                                 new Claim(JwtClaimTypes.Address, @"{ 'street_address': 'One Hacker Way', 'locality': 'Heidelberg', 'postal_code': 69118, 'country': 'Germany' }", IdentityServer4.IdentityServerConstants.ClaimValueTypes.Json)
                        // }).Result;

                        var validate = _signInManager.PasswordlessSignInAsync(user, true, lockoutOnFailure: true);

                        await _events.RaiseAsync(new UserLoginSuccessEvent(user.Email, user.Id, user.Email));
                        
                        // request for a local page
                        if (Url.IsLocalUrl(model.ReturnUrl))
                        {
                            return Redirect(model.ReturnUrl);
                        }
                        else if (string.IsNullOrEmpty(model.ReturnUrl))
                        {
                            return Redirect("~/");
                        }
                        else
                        {
                            // user might have clicked on a malicious link - should be logged
                            throw new Exception("invalid return URL");
                        }
                    }

                    await _events.RaiseAsync(new UserLoginFailureEvent(model.Email, "Invalid Email Address"));
                    ModelState.AddModelError(string.Empty, AccountOptions.InvalidCredentialsErrorMessage);
                }

                return RedirectToAction("Index", new {
                    error = "Something went wrong - 01"
                });
        }


        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> IndexLegacy(PasswordlessLoginModel model, string button)
        // {
        //     // check if we are in the context of an authorization request
        //     var context = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);
        //     // the user clicked the "cancel" button
        //     if (button != "login")
        //     {
        //         if (context != null)
        //         {
        //             // if the user cancels, send a result back into IdentityServer as if they 
        //             // denied the consent (even if this client does not require consent).
        //             // this will send back an access denied OIDC error response to the client.
        //             await _interaction.GrantConsentAsync(context, ConsentResponse.Denied);

        //             // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
        //             //if (await _clientStore.IsPkceClientAsync(context.ClientId))
        //             //{
        //                 // if the client is PKCE then we assume it's native, so this change in how to
        //                 // return the response is for better UX for the end user.
        //                 //return View("Redirect", new RedirectViewModel { RedirectUrl = model.ReturnUrl });
        //             //}

        //             return Redirect(model.ReturnUrl);
        //         }
        //         else
        //         {
        //             // since we don't have a valid context, then we just go back to the home page
        //             return Redirect("~/");
        //         }
        //     }
        //     if(ModelState.IsValid)
        //     {
        //         var result = await _signInManager.PasswordlessSignInAsync(model.Email, model.ReturnUrl);
        //         return RedirectToAction("Index", new
        //         {
        //             success = "Use the link below to login:",
        //             link = result
        //         });
        //     }

        //     return View(model);
        // }

        // public async Task<IActionResult> Confirm(string email, string token, string returnUrl)
        // {
        //     // check if we are in the context of an authorization request
        //     var context = await _interaction.GetAuthorizationContextAsync(returnUrl); 

        //     var result = await _signInManager.PasswordlessSignInAsync(email, token, true);
        //     if(!result.Succeeded)
        //     {
        //         return RedirectToAction("Index", new
        //         {
        //             error = "This login confirmation link is invalid. Try logging in again."
        //         });
        //     }

        //     if(!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
        //     {
        //         return Redirect(returnUrl);
        //     }

        //     return RedirectToAction("Index", "Home");
        // }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", new
            {
                success = "You have been logged out."
            });
        }
    }

}
