using AutoMapper;
using CodeskLibrary.Models;
using CodeskWeb.Models;
using System;

namespace CodeskWeb.Mappers
{
    public class SessionCard_Session : Profile
    {
        public SessionCard_Session()
        {
            CreateMap<Session, SessionCard>()
                .ForMember(d => d.FileIds, o => o.MapFrom(s => s.FileIds.Split(',', StringSplitOptions.None)))
                .ForMember(d => d.FileTitles, o => o.MapFrom(s => s.FileTitles.Split('$', StringSplitOptions.None)))
                .ForMember(d => d.Participants, o => o.MapFrom(s => s.Participants.Split(',', StringSplitOptions.None)));
        }
    }
}