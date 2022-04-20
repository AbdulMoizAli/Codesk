using CodeskLibrary.DataAccess;
using CodeskWeb.HubModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CodeskWeb.Areas.WorkSpace.Controllers
{
    public class EditorController : Controller
    {
        [HttpPost]
        public async Task<IActionResult> SaveUserEditorSetting(int settingId, string settingValue)
        {
            var email = User.GetEmailAddress();

            await EditorManager.SaveUserEditorSetting(email, settingId, settingValue).ConfigureAwait(false);

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> ResetEditorSettings()
        {
            var email = User.GetEmailAddress();

            await EditorManager.ResetEditorSettings(email).ConfigureAwait(false);

            return Ok();
        }

        [AllowAnonymous]
        public IActionResult SetCodingMode(bool mode, string sessionKey, string userId)
        {
            var user = SessionInformation.SessionInfo[sessionKey].connectedUsers.Find(u => u.UserId == userId);
            user.IsPrivateMode = mode;

            return Ok();
        }
    }
}