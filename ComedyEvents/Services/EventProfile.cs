﻿using AutoMapper;
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
            //this.CreateMap<Event, EventDto>().ReverseMap();
        }
    }
}
