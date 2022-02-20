using AutoMapper;
using CodeskLibrary.Models;
using CodeskWeb.Areas.WorkSpace.Models;

namespace CodeskWeb.Mappers
{
    public class SessionFileViewModel_SessionFile : Profile
    {
        public SessionFileViewModel_SessionFile()
        {
            CreateMap<SessionFile, SessionFileViewModel>();
        }
    }
}