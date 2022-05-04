using AutoMapper;
using CodeskLibrary.DataAccess;
using CodeskLibrary.Models;
using CodeskWeb.Areas.WorkSpace.Models;
using CodeskWeb.HubModels;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CodeskWeb.Areas.WorkSpace.Controllers
{
    public class SessionTaskController : Controller
    {
        private readonly IMapper _mapper;

        public SessionTaskController(IMapper mapper)
        {
            _mapper = mapper;
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

            return Ok(new { TaskId = response });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateTask([FromBody] SessionTaskViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var email = User.GetEmailAddress();

            var sessionTask = _mapper.Map<SessionTask>(model);

            var response = await SessionTaskManager.UpdateTask(email, sessionTask);

            if (response == -1)
                return Unauthorized();

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
    }
}