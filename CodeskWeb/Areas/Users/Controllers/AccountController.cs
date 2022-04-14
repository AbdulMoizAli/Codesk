using AutoMapper;
using CodeskLibrary.DataAccess;
using CodeskLibrary.Models;
using CodeskWeb.Areas.Users.Models;
using GoogleReCaptcha.V3.Interface;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using CodeskWeb.Services;
using CodeskWeb.ServiceModels;

namespace CodeskWeb.Areas.Users.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly IMapper _mapper;

        private readonly ICaptchaValidator _captchaValidator;

        private readonly IConfiguration _configuration;

        private readonly IEmailService _emailService;

        public AccountController(IMapper mapper, ICaptchaValidator captchaValidator, IConfiguration configuration, IEmailService emailService)
        {
            _mapper = mapper;
            _captchaValidator = captchaValidator;
            _configuration = configuration;
            _emailService = emailService;
        }

        public IActionResult SignIn()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Dashboard", "Home", new { Area = "" });

            return View();
        }

        public IActionResult SignUp()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Dashboard", "Home", new { Area = "" });

            return View();
        }

        public async Task<IActionResult> ValidateEmailAddress(string emailAddress)
        {
            var result = await AccountManager.IsUniqueEmailAddress(emailAddress)
                .ConfigureAwait(false);

            return Json(result);
        }

        private async Task<ActionResult> AuthorizeUser(User user, bool rememberMe, string url)
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
                new ClaimsPrincipal(claimsIdentity),
                new AuthenticationProperties { IsPersistent = rememberMe })
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

            return await AuthorizeUser(userModel, model.RememberMe, returnUrl).ConfigureAwait(false);
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

            return await AuthorizeUser(userModel, false, null).ConfigureAwait(false);
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
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Dashboard", "Home", new { Area = "" });

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

                var request = new EmailRequest
                {
                    Email = model.EmailAddress,
                    Link = passwordResetLink,
                    Type = "reset",
                    Secret = _configuration["EmailService:AudienceSecret"]
                };

                await _emailService.SendEmail(request).ConfigureAwait(false);
            }

            return View("ForgotPasswordResponse");
        }

        public IActionResult ResetPassword(string token)
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Dashboard", "Home", new { Area = "" });

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