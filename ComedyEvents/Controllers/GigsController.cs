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
    public class GigsController : ControllerBase
    {
        private readonly IEventRepository _eventRepository;
        private readonly IMapper _mapper;
        private readonly LinkGenerator _linkGenerator;

        public GigsController(IEventRepository eventRepository, IMapper mapper, LinkGenerator linkGenerator)
        {
            _eventRepository = eventRepository;
            _mapper = mapper;
            _linkGenerator = linkGenerator;
        }

        [HttpGet("searchByEvent")]
        public async Task<ActionResult<GigDto[]>> GetGigsByEvent(int eventId, bool includeComedians = false)
        {
            try
            {
                var results = await _eventRepository.GetGigsByEvent(eventId, includeComedians);

                if (!results.Any()) return NotFound();

                var mappedEntities = _mapper.Map<GigDto[]>(results);
                return Ok(mappedEntities);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }

        [HttpGet("searchByVenue")]
        public async Task<ActionResult<GigDto[]>> GetGigsByVenue(int venueId, bool includeComedians = false)
        {
            try
            {
                var results = await _eventRepository.GetGigsByVenue(venueId, includeComedians);

                if (!results.Any()) return NotFound();

                var mappedEntities = _mapper.Map<GigDto[]>(results);
                return Ok(mappedEntities);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }

        [HttpGet("{gigId}")]
        public async Task<ActionResult<GigDto>> Get(int gigId, bool includeComedians = false)
        {
            try
            {
                var result = await _eventRepository.GetGig(gigId, includeComedians);

                if (result == null) return NotFound();

                var mappedEntity = _mapper.Map<GigDto>(result);
                return Ok(mappedEntity);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }

        [HttpPost]
        public async Task<ActionResult<GigDto>> Post(GigDto dto)
        {
            try
            {
                var mappedEntity = _mapper.Map<Gig>(dto);
                _eventRepository.Add(mappedEntity);

                if (await _eventRepository.Save())
                {
                    var location = _linkGenerator.GetPathByAction("Get", "Gigs", new { mappedEntity.GigId });
                    return Created(location, _mapper.Map<GigDto>(mappedEntity));
                }
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }

            return BadRequest();
        }

        [HttpPut("{gigId}")]
        public async Task<ActionResult<GigDto>> Put(int gigId, GigDto dto)
        {
            try
            {
                var oldGig = await _eventRepository.GetGig(gigId);
                if (oldGig == null) return NotFound($"Could not find gig with id {gigId}");

                var newGig = _mapper.Map(dto, oldGig);
                _eventRepository.Update(newGig);
                if (await _eventRepository.Save())
                {
                    return NoContent();
                }
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }

            return BadRequest();
        }

        [HttpDelete("{gigId}")]
        public async Task<IActionResult> Delete(int gigId)
        {
            try
            {
                var oldGig = await _eventRepository.GetGig(gigId);
                if (oldGig == null) return NotFound($"Could not find gig with id {gigId}");

                _eventRepository.Delete(oldGig);
                if (await _eventRepository.Save())
                {
                    return NoContent();
                }
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }

            return BadRequest();
        }
    }
}
