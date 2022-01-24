using CodeskLibrary.DataAccess;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.IO;
using CodeFile = System.IO.File;
using CodeskWeb.HubModels;
using System.Security.Claims;
using System.Linq;
using System.IO.Compression;

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
        public async Task<IActionResult> UpdateFileContent([FromQuery] string filePath, [FromQuery] string sessionKey, [FromBody] string fileContent)
        {
            var path = Path.Combine(_env.WebRootPath, "assets", "session", "files", filePath);

            if (!CodeFile.Exists(path))
                return BadRequest();

            SessionInformation.SessionInfo[sessionKey].code.Clear();
            SessionInformation.SessionInfo[sessionKey].code.Append(fileContent);

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

        public async Task<IActionResult> DownloadSessionFile(int fileId)
        {
            var email = User.FindFirst(x => x.Type == ClaimTypes.Email).Value;

            var sessionFile = await SessionFileManager.DownloadSessionFile(email, fileId).ConfigureAwait(false);

            var path = Path.Combine(_env.WebRootPath, "assets", "session", "files", sessionFile.FilePath);

            var memoryStream = new MemoryStream();

            using var fileStream = new FileStream(path, FileMode.Open);

            await fileStream.CopyToAsync(memoryStream);

            memoryStream.Position = 0;

            return File(memoryStream, "text/plain", $"{sessionFile.FileTitle}.{sessionFile.SessionFileType.FileTypeExtension}");
        }

        public async Task<IActionResult> DownloadSessionFiles(int sessionId)
        {
            var email = User.FindFirst(x => x.Type == ClaimTypes.Email).Value;

            var sessionFiles = await SessionFileManager.DownloadSessionFiles(email, sessionId).ConfigureAwait(false);

            var archive = Path.Combine(_env.WebRootPath, "assets", "session", "archive.zip");
            var temp = Path.Combine(_env.WebRootPath, "assets", "session", "temp");

            if (CodeFile.Exists(archive))
                CodeFile.Delete(archive);

            Directory.EnumerateFiles(temp).ToList().ForEach(f => CodeFile.Delete(f));

            sessionFiles.ForEach(f => CodeFile.Copy(Path.Combine(_env.WebRootPath, "assets", "session", "files", f.FilePath), Path.Combine(temp, $"{f.FileTitle}.{f.SessionFileType.FileTypeExtension}")));

            ZipFile.CreateFromDirectory(temp, archive);

            var memoryStream = new MemoryStream();

            using var fileStream = new FileStream(archive, FileMode.Open);

            await fileStream.CopyToAsync(memoryStream);

            memoryStream.Position = 0;

            return File(memoryStream, "application/zip", "archive.zip");
        }
    }
}