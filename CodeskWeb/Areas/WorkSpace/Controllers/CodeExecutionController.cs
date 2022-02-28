using CodeskWeb.ServiceModels;
using CodeskWeb.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace CodeskWeb.Areas.WorkSpace.Controllers
{
    public class CodeExecutionController : Controller
    {
        private readonly IConfiguration _configuration;

        private readonly ICodeExecutionService _executionService;

        public CodeExecutionController(IConfiguration configuration, ICodeExecutionService executionService)
        {
            _configuration = configuration;
            _executionService = executionService;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Execute([FromQuery] string language, [FromBody] string code)
        {
            var request = new CodeExecutionRequest
            {
                ClientId = _configuration["CodeExecution:ClientId"],
                ClientSecret = _configuration["CodeExecution:ClientSecret"],
                Language = _configuration[$"CodeExecution:{language}:Language"],
                Script = code,
                VersionIndex = _configuration[$"CodeExecution:{language}:VersionIndex"]
            };

            var output = await _executionService.ExecuteCode(request);

            if (output is null)
                return StatusCode(500);

            return Ok(output);
        }
    }
}