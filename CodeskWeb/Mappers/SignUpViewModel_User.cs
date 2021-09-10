using AutoMapper;
using CodeskLibrary.Models;
using CodeskWeb.Areas.Users.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeskWeb.Mappers
{
    public class SignUpViewModel_User : Profile
    {
        public SignUpViewModel_User()
        {
            CreateMap<SignUpViewModel, User>()
                .ForMember(
                    d => d.PasswordText,
                    o => o.MapFrom(s => s.Password));
        }
    }
}