using AutoMapper;
using CodeskLibrary.DataAccess;
using CodeskLibrary.Models;
using CodeskWeb.Areas.Users.Models;
using FluentEmail.Core;
using GoogleReCaptcha.V3.Interface;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CodeskWeb.Areas.Users.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly IMapper _mapper;

        private readonly IFluentEmail _email;

        private readonly IWebHostEnvironment _env;

        private readonly ICaptchaValidator _captchaValidator;

        public AccountController(IMapper mapper, IFluentEmail email, IWebHostEnvironment env, ICaptchaValidator captchaValidator)
        {
            _mapper = mapper;
            _email = email;
            _env = env;
            _captchaValidator = captchaValidator;
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

        private async Task<ActionResult> AuthorizeUser(User user, string url)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.FullName),
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

            return RedirectToAction("Dashboard", "Home", new { Area = "" });
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

        public async Task<IActionResult> SignInExternalCallback(string returnUrl, string remoteError)
        {
            if (remoteError != null)
            {
                ModelState.AddModelError("SignInFailed", remoteError);
                return View("SignIn");
            }

            string url = string.IsNullOrWhiteSpace(returnUrl) ? Url.Action("Dashboard", "Home", new { Area = "" })
                : returnUrl;

            var email = User.FindFirst(x => x.Type == ClaimTypes.Email).Value;

            var result = await AccountManager.IsUniqueEmailAddress(email).ConfigureAwait(false);

            if (!result)
            {
                return LocalRedirect(url);
            }

            await AccountManager.UserExternalSignUp(email).ConfigureAwait(false);

            return LocalRedirect(url);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp(SignUpViewModel model, string captcha)
        {
            if (!await _captchaValidator.IsCaptchaPassedAsync(captcha).ConfigureAwait(false))
            {
                ModelState.AddModelError("captcha", "Captcha validation failed");
                return View(model);
            }

            if (!ModelState.IsValid)
                return View(model);

            var userModel = _mapper.Map<User>(model);

            var response = await AccountManager.UserSignUp(userModel)
                .ConfigureAwait(false);

            if (response == -1)
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

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var token = await AccountManager.GetForgotPasswordToken(model.EmailAddress)
                .ConfigureAwait(false);

            if (token != null)
            {
                var passwordResetLink = Url.Action("ResetPassword", "Account", new { Area = "Users", token }, Request.Scheme);

                var templateFilePath = $"{_env.WebRootPath}\\EmailTemplates\\PasswordReset.html";

                await _email
                    .To(model.EmailAddress)
                    .Subject("Password Reset - Codesk")
                    .UsingTemplateFromFile(templateFilePath, new { Link = passwordResetLink }, true)
                    .SendAsync()
                    .ConfigureAwait(false);
            }

            return View("ForgotPasswordResponse");
        }

        public IActionResult ResetPassword(string token)
        {
            var model = new ResetPasswordViewModel { Token = token };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await AccountManager.ResetPassword(model.Password, model.Token).ConfigureAwait(false);

            return RedirectToAction("SignIn", "Account", new { Area = "Users" });
        }
    }
}