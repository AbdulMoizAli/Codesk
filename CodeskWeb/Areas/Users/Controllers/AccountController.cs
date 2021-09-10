using AutoMapper;
using CodeskLibrary.DataAccess;
using CodeskLibrary.Models;
using CodeskWeb.Areas.Users.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CodeskWeb.Areas.Users.Controllers
{
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

            return RedirectToAction("Index", "Home", new { Area = "" });
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
    }
}