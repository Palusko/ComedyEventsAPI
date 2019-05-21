using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ComedyEvents.Context;
using ComedyEvents.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ComedyEvents.Services
{
    public class EventRepository : IEventRepository
    {
        private readonly EventContext _eventContext;
        private readonly ILogger<EventRepository> _logger;

        public EventRepository(EventContext eventContext, ILogger<EventRepository> logger)
        {
            _eventContext = eventContext;
            _logger = logger;
        }

        public void Add<T>(T entity) where T : class
        {
            _logger.LogInformation($"Adding object of type {entity.GetType()}");
            _eventContext.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _logger.LogInformation($"Deleting object of type {entity.GetType()}");
            _eventContext.Remove(entity);
        }

        public async Task<Comedian> GetComedian(int comedianId)
        {
            _logger.LogInformation($"Getting comedian for id {comedianId}");

            var query = _eventContext.Comedians
                        .Where(c => c.ComedianId == comedianId);

            return await query.FirstOrDefaultAsync();
        }

        public async Task<Comedian[]> GetComedians()
        {
            _logger.LogInformation($"Getting all comedians");
            var query = _eventContext.Comedians
                        .OrderBy(c => c.LastName);

            return await query.ToArrayAsync();
        }

        public async Task<Comedian[]> GetComediansByEvent(int eventId)
        {
            _logger.LogInformation($"Getting all comedians for event for event id {eventId}");
            IQueryable<Comedian> query = _eventContext.Gigs
                                        .Where(c => c.Event.EventId == eventId)
                                        .Select(c => c.Comedian)
                                        .OrderBy(c => c.LastName);

            return await query.ToArrayAsync();
        }

        public async Task<Event> GetEvent(int eventId, bool includeGigs = false)
        {
            _logger.LogInformation($"Getting event for event id {eventId}");

            IQueryable<Event> query = _eventContext.Events
                                    .Include(v => v.Venue);

            if (includeGigs)
            {
                query = query.Include(g => g.Gigs)
                             .ThenInclude(c => c.Comedian);
            }

            query = query.Where(e => e.EventId == eventId);

            return await query.FirstOrDefaultAsync();
        }

        public async Task<Event[]> GetEvents(bool includeGigs = false)
        {
            _logger.LogInformation($"Getting events");

            IQueryable<Event> query = _eventContext.Events
                                    .Include(v => v.Venue);

            if (includeGigs)
            {
                query = query.Include(g => g.Gigs)
                             .ThenInclude(c => c.Comedian);
            }

            query = query.OrderBy(e => e.EventDate);

            return await query.ToArrayAsync();
        }

        public async Task<Event[]> GetEventsByDate(DateTime date, bool includeGigs = false)
        {
            _logger.LogInformation($"Getting events for date {date}");

            IQueryable<Event> query = _eventContext.Events
                                    .Include(v => v.Venue);

            if (includeGigs)
            {
                query = query.Include(g => g.Gigs)
                             .ThenInclude(c => c.Comedian);
            }

            query = query.OrderBy(e => e.EventDate)
                        .Where(e => e.EventDate == date);                          

            return await query.ToArrayAsync();
        }

        public async Task<Gig> GetGig(int gigId, bool includeComedians = false)
        {
            _logger.LogInformation($"Getting gig with id {gigId}");
            IQueryable<Gig> query = _eventContext.Gigs;

            if(includeComedians)
            {
                query = query.Include(c => c.Comedian);
            }

            query = query.Where(g => g.GigId == gigId).Include(e => e.Event);
            return await query.FirstOrDefaultAsync();
        }

        public async Task<Gig[]> GetGigsByEvent(int eventId, bool includeComedians = false)
        {
            _logger.LogInformation($"Getting gigs for event id {eventId}");
            IQueryable<Gig> query = _eventContext.Gigs;

            if (includeComedians)
            {
                query = query.Include(c => c.Comedian);
            }

            query = query.Where(e => e.Event.EventId == eventId)
                        .Include(e => e.Event)
                        .OrderByDescending(g => g.GigHeadline);

            return await query.ToArrayAsync();
        }

        public async Task<Gig[]> GetGigsByVenue(int venueId, bool includeComedians = false)
        {
            _logger.LogInformation($"Getting gigs for venue id {venueId}");
            IQueryable<Gig> query = _eventContext.Gigs;

            if (includeComedians)
            {
                query = query.Include(c => c.Comedian);
            }

            query = query.Where(v => v.Event.Venue.VenueId == venueId)
                        .Include(v => v.Event.Venue)
                        .OrderByDescending(g => g.GigHeadline);

            return await query.ToArrayAsync();
        }

        public async Task<bool> Save()
        {
            _logger.LogInformation("Saving changes");
            return (await _eventContext.SaveChangesAsync()) >= 0;
        }

        public void Update<T>(T entity) where T : class
        {
            _logger.LogInformation($"Updating object of type {entity.GetType()}");
            _eventContext.Update(entity);
        }
    }
}
