using CodeskLibrary.DataAccess;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CodeskWeb.Areas.WorkSpace.Controllers
{
    public class EditorController : Controller
    {
        [HttpPost]
        public async Task<IActionResult> SaveUserEditorSetting(int settingId, string settingValue)
        {
            var email = User.FindFirst(x => x.Type == ClaimTypes.Email).Value;

            await EditorManager.SaveUserEditorSetting(email, settingId, settingValue).ConfigureAwait(false);

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> ResetEditorSettings()
        {
            var email = User.FindFirst(x => x.Type == ClaimTypes.Email).Value;

            await EditorManager.ResetEditorSettings(email).ConfigureAwait(false);

            return Ok();
        }
    }
}