using CodeskLibrary.DataAccess;
using CodeskWeb.Areas.WorkSpace.Models;
using CodeskWeb.HubModels;
using CodeskWeb.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace CodeskWeb.Areas.WorkSpace.Controllers
{
    public class SessionController : Controller
    {
        private readonly IHubContext<SessionHub, ISessionClient> _hubContext;

        public SessionController(IHubContext<SessionHub, ISessionClient> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task<IActionResult> NewSession()
        {
            var email = User.GetEmailAddress();

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
            var email = User.GetEmailAddress();

            await SessionManager.SaveSession(email, startDateTime, sessionKey).ConfigureAwait(false);

            return Ok();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> SaveParticipant(string userName, string sessionKey)
        {
            var participantId = await SessionManager.SaveParticipant(userName, sessionKey).ConfigureAwait(false);

            return Ok(new { ParticipantId = participantId });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSession(int sessionId)
        {
            await SessionManager.DeleteSession(sessionId).ConfigureAwait(false);

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
                email = User.GetEmailAddress();

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

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ReconnectSessionUser([FromBody] ReconnectViewModel model)
        {
            await _hubContext.Groups.AddToGroupAsync(model.User.UserId, model.SessionKey)
                .ConfigureAwait(false);

            await _hubContext.Clients.Group(model.SessionKey).UpdateUserId(model.PreviousUserId, model.User.UserId)
                .ConfigureAwait(false);

            SessionInformation.SessionInfo[model.SessionKey].connectedUsers.Insert(0, model.User);

            if (model.IsHost)
            {
                var (language, code, _, connectedUsers) = SessionInformation.SessionInfo[model.SessionKey];
                SessionInformation.SessionInfo[model.SessionKey] = (language, code, model.User.UserId, connectedUsers);
            }

            return Ok();
        }
    }
}