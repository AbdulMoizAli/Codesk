using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeskWeb.Areas.Users.Controllers
{
    public class SessionController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult NewSession()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult JoinSession()
        {
            return View();
        }
    }
}