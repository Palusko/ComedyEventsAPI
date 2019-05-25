using AutoMapper;
using ComedyEvents.Dto;
using ComedyEvents.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComedyEvents.Services
{
    public class EventProfile : Profile
    {
        public EventProfile()
        {
            CreateMap<Event, EventDto>()
                .ReverseMap()
                .ForMember(v => v.Venue, o => o.Ignore());

            CreateMap<Gig, GigDto>()
                .ReverseMap()
                .ForMember(e => e.Event, o => o.Ignore())
                .ForMember(c => c.Comedian, o => o.Ignore());
        }
    }
}
