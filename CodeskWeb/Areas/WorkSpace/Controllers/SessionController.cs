using CodeskLibrary.DataAccess;
using CodeskWeb.Areas.WorkSpace.Models;
using CodeskWeb.HubModels;
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

            var data = await SessionManager.GetEditorSettings(email).ConfigureAwait(false);

            ViewBag.IsSession = true;

            return View(new SessionViewModel { Settings = data.Item1, Themes = data.Item2, UserSettings = data.Item3 });
        }

        [HttpPost]
        public async Task<IActionResult> SaveUserEditorSetting(int settingId, string settingValue)
        {
            var email = User.FindFirst(x => x.Type == ClaimTypes.Email).Value;

            await SessionManager.SaveUserEditorSetting(email, settingId, settingValue).ConfigureAwait(false);

            return Ok();
        }

        [AllowAnonymous]
        public IActionResult JoinSession()
        {
            var model = new JoinSessionViewModel();

            if (User.Identity.IsAuthenticated)
                model.UserName = User.Identity.Name;

            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> JoinSession(JoinSessionViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (!SessionInformation.SessionInfo.ContainsKey(model.SessionKey))
            {
                ModelState.AddModelError("SessionKey", "Invalid session key");
                return View(model);
            }

            string email = null;

            if (User.Identity.IsAuthenticated)
                email = User.FindFirst(x => x.Type == ClaimTypes.Email).Value;

            var data = await SessionManager.GetEditorSettings(email).ConfigureAwait(false);

            var viewModel = new SessionViewModel
            {
                Settings = data.Item1,
                Themes = data.Item2,
                UserSettings = data.Item3,
                JoinSession = model
            };

            ViewBag.IsSession = true;

            return View("_JoinSession", viewModel);
        }
    }
}