using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeskWeb.Controllers
{
    [AllowAnonymous]
    public class ErrorController : Controller
    {
        public IActionResult InternalServerError()
        {
            return View();
        }

        public IActionResult PageNotFound()
        {
            return View();
        }
    }
}