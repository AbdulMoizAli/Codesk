﻿using CodeskLibrary.DataAccess;
using CodeskWeb.Areas.WorkSpace.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CodeskWeb.Areas.WorkSpace.Controllers
{
    public class SessionController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }

        public async Task<IActionResult> NewSession()
        {
            var email = User.FindFirst(x => x.Type == ClaimTypes.Email).Value;

            var result = await EmailManager.IsEmailConfirmed(email).ConfigureAwait(false);

            if (!result)
                return RedirectToAction("Dashboard", "Session", new { Area = "WorkSpace" });

            var data = await SessionManager.GetEditorSettings().ConfigureAwait(false);

            return View(new SessionViewModel { Settings = data.Item1, Themes = data.Item2 });
        }

        [AllowAnonymous]
        public IActionResult JoinSession()
        {
            return View();
        }
    }
}