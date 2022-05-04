using AutoMapper;
using CodeskLibrary.Models;
using CodeskWeb.Areas.WorkSpace.Models;

namespace CodeskWeb.Mappers
{
    public class SessionTaskViewModel_SessionTask : Profile
    {
        public SessionTaskViewModel_SessionTask()
        {
            CreateMap<SessionTaskViewModel, SessionTask>();
        }
    }
}