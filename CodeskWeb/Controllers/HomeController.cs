using AutoMapper;
using CodeskLibrary.DataAccess;
using CodeskWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CodeskWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMapper _mapper;

        public HomeController(IMapper mapper)
        {
            _mapper = mapper;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Dashboard()
        {
            var email = User.FindFirst(x => x.Type == ClaimTypes.Email).Value;

            var sessions = await SessionManager.GetSessions(email).ConfigureAwait(false);

            var sessionCards = _mapper.Map<List<SessionCard>>(sessions);

            return View(sessionCards);
        }
    }
}