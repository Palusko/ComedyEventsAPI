using AutoMapper;
using ComedyEvents.Dto;
using ComedyEvents.Models;
using ComedyEvents.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComedyEvents.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly IEventRepository _eventRepository;
        private readonly IMapper _mapper;
        private readonly LinkGenerator _linkGenerator;

        public EventsController(IEventRepository eventRepository, IMapper mapper, LinkGenerator linkGenerator)
        {
            _eventRepository = eventRepository;
            _mapper = mapper;
            _linkGenerator = linkGenerator;
        }

        [HttpGet]
        public async Task<ActionResult<EventDto[]>> Get (bool includeGigs = false)
        {
            try
            {
                var results = await _eventRepository.GetEvents(includeGigs);

                var mappedEntities = _mapper.Map<EventDto[]>(results);
                //var eventDto = new EventDto();
                //foreach(var e in results)
                //{
                //    eventDto.City = e.Venue.City;
                //    eventDto.EventDate = e.EventDate;
                //    eventDto.EventName = e.EventName;
                //}
                return Ok(mappedEntities);
            }
            catch(Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }

        [HttpGet("{eventId}")]
        public async Task<ActionResult<EventDto>> Get(int eventId, bool includeGigs = false)
        {
            try
            {
                var result = await _eventRepository.GetEvent(eventId, includeGigs);

                if (result == null) return NotFound();

                var mappedEntity = _mapper.Map<EventDto>(result);
                return Ok(mappedEntity);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<EventDto[]>> SearchByDate (DateTime date, bool includeGigs = false)
        {
            try
            {
                var results = await _eventRepository.GetEventsByDate(date, includeGigs);

                if (!results.Any()) return NotFound();

                var mappedEntities = _mapper.Map<EventDto[]>(results);
                return Ok(mappedEntities);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }

        [HttpPost]
        public async Task<ActionResult<EventDto>> Post(EventDto dto)
        {
            try
            {
                
                var mappedEntity = _mapper.Map<Event>(dto);
                _eventRepository.Add(mappedEntity);

                if (await _eventRepository.Save())
                {
                    //return Created($"/api/events/{mappedEntity.EventId}", _mapper.Map<EventDto>(mappedEntity));
                    var location = _linkGenerator.GetPathByAction("Get", "Events", new { eventId = dto.EventId });
                    return Created(location, _mapper.Map<EventDto>(mappedEntity));
                }
            }
            catch (Exception ex)
            {
                
                return this.StatusCode(StatusCodes.Status500InternalServerError, ex.InnerException);
            }

            return BadRequest();
        }
    }
}
