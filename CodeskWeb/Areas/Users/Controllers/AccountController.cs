using AutoMapper;
using CodeskLibrary.DataAccess;
using CodeskLibrary.Models;
using CodeskWeb.Areas.Users.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CodeskWeb.Areas.Users.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly IMapper _mapper;

        public AccountController(IMapper mapper)
        {
            _mapper = mapper;
        }

        public IActionResult SignIn()
        {
            return View();
        }

        public IActionResult SignUp()
        {
            return View();
        }

        public async Task<IActionResult> ValidateEmailAddress(string emailAddress)
        {
            var result = await AccountManager.IsUniqueEmailAddress(emailAddress)
                .ConfigureAwait(false);

            return Json(result);
        }

        public async Task<IActionResult> ValidateUserName(string userName)
        {
            var result = await AccountManager.IsUniqueUserName(userName)
                .ConfigureAwait(false);

            return Json(result);
        }

        private async Task<ActionResult> AuthorizeUser(User user, string url)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.FullName),
                new(ClaimTypes.GivenName, user.UserName),
                new(ClaimTypes.Email, user.EmailAddress)
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity))
                .ConfigureAwait(false);

            if (!string.IsNullOrWhiteSpace(url))
                return LocalRedirect(url);

            return RedirectToAction("Dashboard", "Session", new { Area = "Users" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignIn(SignInViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userModel = await AccountManager.UserSignIn(model.EmailAddress, model.Password)
                .ConfigureAwait(false);

            if (userModel is null)
            {
                ModelState.AddModelError("SignInFailed", "Incorrect email address or password");
                return View(model);
            }

            return await AuthorizeUser(userModel, returnUrl).ConfigureAwait(false);
        }

        public async Task SignInExternal(string returnUrl, string providerName)
        {
            string url = Url.Action("SignInExternalCallback", "Account", new { Area = "Users", ReturnUrl = returnUrl });

            await HttpContext.ChallengeAsync(providerName, new AuthenticationProperties { RedirectUri = url })
                .ConfigureAwait(false);
        }

        public IActionResult SignInExternalCallback(string returnUrl, string remoteError)
        {
            string url = string.IsNullOrWhiteSpace(returnUrl) ? Url.Action("Dashboard", "Session", new { Area = "Users" })
                : returnUrl;

            if (remoteError != null)
            {
                ModelState.AddModelError("SignInFailed", remoteError);
                return View("SignIn");
            }

            // TODO: save user in the database
  
            return LocalRedirect(url);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp(SignUpViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userModel = _mapper.Map<User>(model);

            var response = await AccountManager.UserSignUp(userModel)
                .ConfigureAwait(false);

            if (response == -1)
            {
                ModelState.AddModelError("UserName", "User name is already taken");
                return View(model);
            }

            if (response == -2)
            {
                ModelState.AddModelError("EmailAddress", "Email address is already taken");
                return View(model);
            }

            return await AuthorizeUser(userModel, null).ConfigureAwait(false);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme)
                .ConfigureAwait(false);

            return RedirectToAction("Index", "Home", new { Area = "" });
        }
    }
}