using AutoMapper;
using CodeskLibrary.DataAccess;
using CodeskWeb.HubModels;
using CodeskWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
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
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Dashboard");

            return View();
        }

        public async Task<IActionResult> Dashboard()
        {
            var email = User.GetEmailAddress();

            var sessions = await SessionManager.GetSessions(email).ConfigureAwait(false);

            var sessionCards = _mapper.Map<List<SessionCard>>(sessions);

            return View(sessionCards);
        }
    }
}