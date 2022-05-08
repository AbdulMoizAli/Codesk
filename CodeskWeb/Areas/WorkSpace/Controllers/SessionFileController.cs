using CodeskLibrary.DataAccess;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.IO;
using CodeFile = System.IO.File;
using CodeskWeb.HubModels;
using System.Linq;
using System.IO.Compression;
using AutoMapper;
using CodeskWeb.Areas.WorkSpace.Models;
using System;

namespace CodeskWeb.Areas.WorkSpace.Controllers
{
    public class SessionFileController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;

        public SessionFileController(IWebHostEnvironment env, IMapper mapper)
        {
            _env = env;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> CreateSessionFile(string fileType, string sessionKey, string connectionId)
        {
            if (!SessionHelper.IsValidHostId(connectionId, sessionKey))
                return Unauthorized();

            var sessionFileType = await SessionFileManager.GetFileTypeExtension(fileType).ConfigureAwait(false);

            if (sessionFileType is null)
                return BadRequest();

            var (_, code, hostId, connectedUsers) = SessionInformation.SessionInfo[sessionKey];
            SessionInformation.SessionInfo[sessionKey] = (fileType, code, hostId, connectedUsers);

            var email = User.GetEmailAddress();

            var sessionFile = await SessionFileManager.GetSessionFile(email, sessionKey, sessionFileType.FileTypeId);

            if (sessionFile is not null)
            {
                var path = Path.Combine(_env.WebRootPath, "assets", "session", "files", sessionFile.FilePath);

                if (!CodeFile.Exists(path))
                    return StatusCode(500);

                var fileContent = await CodeFile.ReadAllTextAsync(path).ConfigureAwait(false);

                return Ok(new { SessionCurrentFile = _mapper.Map<SessionFileViewModel>(sessionFile), FileContent = fileContent });
            }

            var fileName = $"{Guid.NewGuid()}_{Path.GetRandomFileName()}.{sessionFileType.FileTypeExtension}";

            var filePath = Path.Combine(_env.WebRootPath, "assets", "session", "files", fileName);

            await CodeFile.Create(filePath).DisposeAsync().ConfigureAwait(false);

            return Ok(new { SessionCurrentFile = _mapper.Map<SessionFileViewModel>(await SessionFileManager.SaveSessionFile(email, sessionKey, fileName, sessionFileType.FileTypeId)), FileContent = string.Empty });
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
        public IActionResult GetSessionCurrentFileInfo(string sessionKey)
        {
            var data = new { Language = SessionInformation.SessionInfo[sessionKey].language, EditorContent = SessionInformation.SessionInfo[sessionKey].code.ToString() };
            return Ok(data);
        }

        public async Task<IActionResult> DownloadSessionFile(int fileId)
        {
            var email = User.GetEmailAddress();

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
            var email = User.GetEmailAddress();

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