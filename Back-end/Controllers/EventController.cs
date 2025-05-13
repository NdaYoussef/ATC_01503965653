using EventManagmentTask.DTOs;
using EventManagmentTask.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EventManagmentTask.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IEventRepository _eventRepository;

        #region Constructor
        public EventController(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        #endregion

        #region EndPoints

        [HttpGet("Events")]
        public async Task<IActionResult> GetAllEvents()
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var response = await _eventRepository.GetAllEvents();
            if (response.IsSucceeded)
                return Ok(response);
            return StatusCode(response.StatusCode, new { response.Message });
        }

        [Authorize(Policy = "Admin and Organizer")]

        [HttpGet("EventById/{id}")]
        public async Task<IActionResult> GetEventsById(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var response = await _eventRepository.GetEventsById(id);
            if (response.IsSucceeded)
                return Ok(response);
            return StatusCode(response.StatusCode, new { response.Message });
        }


        [Authorize(Policy = "Admin and Organizer")]

        [HttpPost("AddEvent")]

        public async Task<IActionResult> AddEvent([FromBody] EventDto eventDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var response = await _eventRepository.AddEvent(eventDto);
            if (response.IsSucceeded)
                return Ok(response);

            return StatusCode(response.StatusCode, new { response.Message });
        }

        [Authorize(Policy = "Admin and Organizer")]

        [HttpPut("EditEvent")]
 
        public async Task<IActionResult> EditEvent([FromBody] EventDto eventDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _eventRepository.EditEvent(eventDto);
            if (response.IsSucceeded)
                return Ok(response);

            return StatusCode(response.StatusCode, new { response.Message });
        }

        [Authorize(Policy = "Admin and Organizer")]

        [HttpDelete("DeleteEvent/{id}")]

        public async Task<IActionResult> DeleteEvent(int id)
        {
            var response = await _eventRepository.DeleteEvent(id);
            if (response.IsSucceeded)
                return Ok(response);

            return StatusCode(response.StatusCode, new { response.Message });
        } 
        #endregion
    }
}
