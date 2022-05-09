using AutoMapper;
using CodeskLibrary.DataAccess;
using CodeskLibrary.Models;
using CodeskWeb.Areas.WorkSpace.Models;
using CodeskWeb.HubModels;
using CodeskWeb.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.IO;
using System.Threading.Tasks;
using SubmissionFile = System.IO.File;

namespace CodeskWeb.Areas.WorkSpace.Controllers
{
    public class SessionTaskController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IHubContext<SessionHub, ISessionClient> _hubContext;
        private readonly IWebHostEnvironment _env;

        public SessionTaskController(IMapper mapper, IHubContext<SessionHub, ISessionClient> hubContext, IWebHostEnvironment env)
        {
            _mapper = mapper;
            _hubContext = hubContext;
            _env = env;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask([FromQuery] string sessionKey, [FromBody] SessionTaskViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var email = User.GetEmailAddress();

            var sessionTask = _mapper.Map<SessionTask>(model);

            var response = await SessionTaskManager.CreateTask(email, sessionKey, sessionTask);

            if (response == -1)
                return Unauthorized();

            model.TaskId = response;

            await _hubContext.Clients.GroupExcept(sessionKey, SessionInformation.SessionInfo[sessionKey].hostId)
                .ReceiveTaskInfo(model, 1)
                .ConfigureAwait(false);

            await _hubContext.Clients.GroupExcept(sessionKey, SessionInformation.SessionInfo[sessionKey].hostId)
                .NotifyUser(NotificationMessage.GetNewTaskMessage())
                .ConfigureAwait(false);

            return Ok(new { TaskId = response });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateTask([FromQuery] string sessionKey, [FromBody] SessionTaskViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var email = User.GetEmailAddress();

            var sessionTask = _mapper.Map<SessionTask>(model);

            var response = await SessionTaskManager.UpdateTask(email, sessionTask);

            if (response == -1)
                return Unauthorized();

            await _hubContext.Clients.GroupExcept(sessionKey, SessionInformation.SessionInfo[sessionKey].hostId)
                .ReceiveTaskInfo(model, 2)
                .ConfigureAwait(false);

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTask(int taskId)
        {
            var email = User.GetEmailAddress();

            var response = await SessionTaskManager.DeleteTask(email, taskId);

            if (response == -1)
                return Unauthorized();

            return Ok();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> SaveSubmission([FromBody] ParticipantTaskSubmissionViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var submission = await SessionTaskManager.GetParticipantTaskSubmission(model.TaskId, model.ParticipantId).ConfigureAwait(false);

            if (submission is not null)
            {
                var path = Path.Combine(_env.WebRootPath, "assets", "session", "files", submission.FilePath);

                if (!SubmissionFile.Exists(path))
                    return StatusCode(500);

                await SubmissionFile.WriteAllTextAsync(path, model.SubmissionText).ConfigureAwait(false);
            }
            else
            {
                var fileName = $"{Guid.NewGuid()}_{Path.GetRandomFileName()}.txt";

                var filePath = Path.Combine(_env.WebRootPath, "assets", "session", "files", fileName);

                await SubmissionFile.Create(filePath).DisposeAsync().ConfigureAwait(false);

                await SubmissionFile.WriteAllTextAsync(filePath, model.SubmissionText).ConfigureAwait(false);

                var newSubmission = new ParticipantTaskSubmission
                {
                    TaskId = model.TaskId,
                    ParticipantId = model.ParticipantId,
                    FilePath = fileName
                };

                await SessionTaskManager.SaveParticipantTaskSubmission(newSubmission).ConfigureAwait(false);
            }

            return Ok();
        }

        [AllowAnonymous]
        public async Task<IActionResult> GetSubmissionText([FromQuery] int taskId, [FromQuery] int participantId)
        {
            var submission = await SessionTaskManager.GetParticipantTaskSubmission(taskId, participantId).ConfigureAwait(false);

            if (submission is null)
                return StatusCode(500);

            var path = Path.Combine(_env.WebRootPath, "assets", "session", "files", submission.FilePath);

            if (!SubmissionFile.Exists(path))
                return StatusCode(500);

            var submissionText = await SubmissionFile.ReadAllTextAsync(path).ConfigureAwait(false);

            return Ok(new { SubmissionText = submissionText });
        }

        public async Task<IActionResult> GetTaskSubmissions(string sessionKey, int taskId)
        {
            var submissions = await SessionTaskManager.GetTaskSubmissions(sessionKey, taskId).ConfigureAwait(false);
            return Ok(submissions);
        }

        public async Task<IActionResult> ReadSubmissionCode(string filePath)
        {
            var path = Path.Combine(_env.WebRootPath, "assets", "session", "files", filePath);

            if (!SubmissionFile.Exists(path))
                return StatusCode(500);

            var submissionCode = await SubmissionFile.ReadAllTextAsync(path).ConfigureAwait(false);

            return Ok(new { SubmissionCode = submissionCode });
        }
    }
}