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
    public class ComediansController : ControllerBase
    {
        private readonly IEventRepository _eventRepository;
        private readonly IMapper _mapper;
        private readonly LinkGenerator _linkGenerator;

        public ComediansController(IEventRepository eventRepository, IMapper mapper, LinkGenerator linkGenerator)
        {
            _eventRepository = eventRepository;
            _mapper = mapper;
            _linkGenerator = linkGenerator;
        }

        [HttpGet]
        public async Task<ActionResult<ComedianDto[]>> Get()
        {
            try
            {
                var results = await _eventRepository.GetComedians();

                var mappedEntities = _mapper.Map<ComedianDto[]>(results);
                return Ok(mappedEntities);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }

        [HttpGet("{comedianId}")]
        public async Task<ActionResult<ComedianDto>> Get(int comedianId)
        {
            try
            {
                var result = await _eventRepository.GetComedian(comedianId);

                if (result == null) return NotFound();

                var mappedEntity = _mapper.Map<ComedianDto>(result);
                return Ok(mappedEntity);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<ComedianDto[]>> GetComediansByEvent(int eventId)
        {
            try
            {
                var results = await _eventRepository.GetComediansByEvent(eventId);

                if (!results.Any()) return NotFound();

                var mappedEntities = _mapper.Map<ComedianDto[]>(results);
                return Ok(mappedEntities);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }

        [HttpPost]
        public async Task<ActionResult<ComedianDto>> Post(ComedianDto dto)
        {
            try
            {
                var mappedEntity = _mapper.Map<Comedian>(dto);
                _eventRepository.Add(mappedEntity);

                if (await _eventRepository.Save())
                {
                    var location = _linkGenerator.GetPathByAction("Get", "Comedians", new { mappedEntity.ComedianId });
                    return Created(location, _mapper.Map<ComedianDto>(mappedEntity));
                }
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }

            return BadRequest();
        }

        [HttpPut("{comedianId}")]
        public async Task<ActionResult<ComedianDto>> Put(int comedianId, ComedianDto dto)
        {
            try
            {
                var oldComedian = await _eventRepository.GetComedian(comedianId);
                if (oldComedian == null) return NotFound($"Could not find comedian with id {comedianId}");

                var newComedian = _mapper.Map(dto, oldComedian);
                _eventRepository.Update(newComedian);
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

        [HttpDelete("{comedianId}")]
        public async Task<IActionResult> Delete(int comedianId)
        {
            try
            {
                var oldComedian = await _eventRepository.GetComedian(comedianId);
                if (oldComedian == null) return NotFound($"Could not find comedian with id {comedianId}");

                _eventRepository.Delete(oldComedian);
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
