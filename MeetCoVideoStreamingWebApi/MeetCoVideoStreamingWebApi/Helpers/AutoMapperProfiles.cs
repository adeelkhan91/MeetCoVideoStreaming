using AutoMapper;
using MeetCoVideoStreamingWebApi.Dtos;
using MeetCoVideoStreamingWebApi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetCoVideoStreamingWebApi.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Users, MemberDto>();

            CreateMap<RegisterDto, Users>();

            CreateMap<Meeting, MeetingDto>()
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.User.DisplayName))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName));
        }
    }
}
