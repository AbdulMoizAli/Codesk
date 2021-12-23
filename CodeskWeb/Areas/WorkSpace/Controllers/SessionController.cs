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
        public async Task<IActionResult> NewSession()
        {
            var email = User.FindFirst(x => x.Type == ClaimTypes.Email).Value;

            var result = await EmailManager.IsEmailConfirmed(email).ConfigureAwait(false);

            if (!result)
                return RedirectToAction("Dashboard", "Home", new { Area = "" });

            var data = await EditorManager.GetEditorSettings(email).ConfigureAwait(false);

            ViewBag.IsSession = true;
            ViewBag.NewSession = true;

            return View(new SessionViewModel { Settings = data.Item1, Themes = data.Item2, UserSettings = data.Item3 });
        }

        [HttpPost]
        public async Task<IActionResult> SaveSession(string startDateTime, string sessionKey)
        {
            var email = User.FindFirst(x => x.Type == ClaimTypes.Email).Value;

            await SessionManager.SaveSession(email, startDateTime, sessionKey).ConfigureAwait(false);

            return Ok();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> SaveParticipant(string userName, string sessionKey)
        {
            await SessionManager.SaveParticipant(userName, sessionKey).ConfigureAwait(false);

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

            var data = await EditorManager.GetEditorSettings(email).ConfigureAwait(false);

            var viewModel = new SessionViewModel
            {
                Settings = data.Item1,
                Themes = data.Item2,
                UserSettings = data.Item3,
                JoinSession = model
            };

            ViewBag.IsSession = true;
            ViewBag.JoinSession = true;

            return View("_JoinSession", viewModel);
        }
    }
}