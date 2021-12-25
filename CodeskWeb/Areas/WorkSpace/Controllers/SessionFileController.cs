using CodeskLibrary.DataAccess;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.IO;
using CodeFile = System.IO.File;

namespace CodeskWeb.Areas.WorkSpace.Controllers
{
    public class SessionFileController : Controller
    {
        private readonly IWebHostEnvironment _env;

        public SessionFileController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpPost]
        public async Task<IActionResult> UpdateFileTitle(int fileId, string fileTitle)
        {
            await SessionFileManager.UpdateFileTitle(fileId, fileTitle);

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateFileContent([FromQuery] string filePath, [FromBody] string fileContent)
        {
            var path = Path.Combine(_env.WebRootPath, "assets", "session", "files", filePath);

            if (!CodeFile.Exists(path))
                return BadRequest();

            await CodeFile.WriteAllTextAsync(path, fileContent).ConfigureAwait(false);

            return Ok();
        }

        [HttpPost]
        public async Task<string> GetFileContent(string filePath)
        {
            var path = Path.Combine(_env.WebRootPath, "assets", "session", "files", filePath);

            if (!CodeFile.Exists(path))
                return null;

            return await CodeFile.ReadAllTextAsync(path).ConfigureAwait(false);
        }
    }
}