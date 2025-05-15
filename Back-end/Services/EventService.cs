using EventManagmentTask.Data;
using EventManagmentTask.DTOs;
using EventManagmentTask.DTOs.EventDTO;
using EventManagmentTask.Interfaces;
using EventManagmentTask.Models;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.Design;
using System.Net;
using System.Security.Claims;

namespace EventManagmentTask.Services
{
    public class EventService : IEventRepository
    {
        #region Dependencies
        private readonly EventManagmentDbContext _context;
        private readonly ILogger<EventService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        #endregion

        #region Constructor
        public EventService(EventManagmentDbContext context, ILogger<EventService> logger, IHttpContextAccessor httpContextAccessor) : base()
        {
            _context = context;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }
        #endregion


        #region Services implementation
        public async Task<ResponseDto> GetAllEvents()
        {
            var events = await _context.Events
                                        .Include(e => e.Category)
                                        .Include(e => e.User)
                                        .Include(e => e.Tags)
                                        .AsNoTracking()
                                        .ToListAsync();
            return new ResponseDto
            {
                Message = events.Any() ? "Events retrived successfully." : "No events found.",
                IsSucceeded = true,
                StatusCode = (int)HttpStatusCode.OK,
                Data = events.Adapt<List<EventDto>>()
            };
        }

        public async Task<ResponseDto> GetEventsById(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid event ID: {EventId}", id);
                return new ResponseDto
                {
                    Message = "Invalid event ID.",
                    IsSucceeded = false,
                    StatusCode = (int)HttpStatusCode.NotFound
                };
            }

            var eventItem = await _context.Events
                                          .Include(e => e.Category)
                                          .Include(e => e.User)
                                          .Include(e => e.Tags)
                                          .AsNoTracking()
                                          .FirstOrDefaultAsync(e => e.Id == id);

            if (eventItem == null)
            {
                return new ResponseDto
                {
                    Message = "Event not found.",
                    IsSucceeded = false,
                    StatusCode = 404
                };
            }

            var eventDto = eventItem.Adapt<EventDto>();
            eventDto.TagIds = eventItem.Tags.Select(t => t.Id).ToList();

            return new ResponseDto
            {
                Message = "Event fetched successfully.",
                IsSucceeded = true,
                StatusCode = 200,
                Data = eventDto
            };
        }

        public async Task<ResponseDto> AddEvent(EventDto eventDto)
        {
            if (string.IsNullOrWhiteSpace(eventDto.Title))
            {
                return new ResponseDto
                {
                    Message = "Event title is required !!",
                    IsSucceeded = false,
                    StatusCode = 400
                };
            }

            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == eventDto.CategoryId);
            if (!categoryExists)
            {
                return new ResponseDto
                {
                    Message = "The specified category does not exist.",
                    IsSucceeded = false,
                    StatusCode = 400
                };
            }


            var userId = _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return new ResponseDto
                {
                    Message = "User is not authenticated.",
                    IsSucceeded = false,
                    StatusCode = 401
                };
            }

            var newEvent = new Event
            {
                Title = eventDto.Title,
                Description = eventDto.Description,
                Image = eventDto.Image,
                Location = eventDto.Location,
                CreatedDate = eventDto.CreatedDate,
                CategoryId = eventDto.CategoryId,
                UserId = userId
            };

            if (eventDto.TagIds != null && eventDto.TagIds.Any())
            {
                newEvent.Tags = await _context.Tags
                    .Where(tag => eventDto.TagIds.Contains(tag.Id))
                    .ToListAsync();
            }

            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();
            var dto = newEvent.Adapt<EventDto>();

            return new ResponseDto
            {
                Message = "Event created successfully.",
                IsSucceeded = true,
                StatusCode = (int)HttpStatusCode.Created,
                Data = dto
            };
        }


        public async Task<ResponseDto> EditEvent(EventDto eventDto, int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid event ID: {EventId}", id);
                return new ResponseDto
                {
                    Message = "Invalid event ID.",
                    IsSucceeded = false,
                    StatusCode = (int)HttpStatusCode.NotFound
                };
            }

            var eventEntity = await _context.Events.FindAsync(id);

            if (eventEntity == null)
            {
                return new ResponseDto
                {
                    Message = "Event not found.",
                    IsSucceeded = false,
                    StatusCode = 404
                };
            }

            // Load tags explicitly (lazy loading assumed)
            await _context.Entry(eventEntity).Collection(e => e.Tags).LoadAsync();

            eventEntity = eventDto.Adapt(eventEntity);

            // Update tags
            eventEntity.Tags.Clear();
            foreach (var tagId in eventDto.TagIds)
            {
                var tag = await _context.Tags.FindAsync(tagId);
                if (tag != null)
                {
                    eventEntity.Tags.Add(tag);
                }
            }

            await _context.SaveChangesAsync();

            var updatedDto = eventEntity.Adapt<EventDto>();

            return new ResponseDto
            {
                Message = "Event updated successfully.",
                IsSucceeded = true,
                StatusCode = (int)HttpStatusCode.OK,
                Data = updatedDto
            };
        }


        public async Task<ResponseDto> DeleteEvent(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid event ID: {EventId}", id);
                return new ResponseDto
                {
                    Message = "Invalid event ID.",
                    IsSucceeded = false,
                    StatusCode = (int)HttpStatusCode.NotFound
                };
            }


            var eventEntity = await _context.Events
                                              .FirstOrDefaultAsync(e => e.Id == id);

            if (eventEntity == null)
            {
                return new ResponseDto
                {
                    Message = "Event not found.",
                    IsSucceeded = false,
                    StatusCode = 404
                };
            }

            _context.Events.Remove(eventEntity);
            await _context.SaveChangesAsync();

            return new ResponseDto
            {
                Message = "Event deleted successfully.",
                IsSucceeded = true,
                StatusCode = 200
            };
        } 
        #endregion


    }
}
