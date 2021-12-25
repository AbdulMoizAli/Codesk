using CodeskLibrary.DataAccess;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CodeskWeb.Areas.WorkSpace.Controllers
{
    public class SessionFileController : Controller
    {
        [HttpPost]
        public async Task<IActionResult> UpdateFileTitle(int fileId, string fileTitle)
        {
            await SessionFileManager.UpdateFileTitle(fileId, fileTitle);

            return Ok();
        }
    }
}