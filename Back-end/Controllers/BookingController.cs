using EventManagmentTask.DTOs;
using EventManagmentTask.Helpers;
using EventManagmentTask.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EventManagmentTask.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingRepository _bookingRepository;

        #region Constructor
        public BookingController(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }
        #endregion

        #region EndPoints

        [Authorize(Policy = "Admin and Organizer")]

        [HttpGet("Bookings")]
        public async Task<IActionResult> GetAllBookings()
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var response = await _bookingRepository.GetAllBookingsAsync();
            if (response.IsSucceeded)
                return Ok(response);
            return StatusCode(response.StatusCode, new { response.Message });
        }

        [Authorize(Policy = "Admin and Organizer")]

        [HttpGet("BookingsByEventId/{eventId}")]
        public async Task<IActionResult> GetBookingByEvent(int eventId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var response = await _bookingRepository.GetBookingByEventAsync(eventId);
            if (response.IsSucceeded)
                return Ok(response);
            return StatusCode(response.StatusCode, new { response.Message });
        }

        [Authorize(Policy = "Admin")]

        [HttpGet("GetUserBookings")]
        public async Task<IActionResult> GetUserBookings(string userId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var response = await _bookingRepository.GetUserBookingsAsync(userId);
            if (response.IsSucceeded)
                return Ok(response);
            return StatusCode(response.StatusCode, new { response.Message });
        }

  //      [Authorize(Policy = "CLient")]

        [HttpPost("CreateBooking")]
        public async Task<IActionResult> CreateBooking(BookingDto booking)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var response = await _bookingRepository.CreateBookingAsync(booking);
            if (response.IsSucceeded)
                return Ok(response);
            return StatusCode(response.StatusCode, new { response.Message });
        }

        [Authorize(Policy = "Admin")]

        [HttpPut("EditBooking")]
        public async Task<IActionResult> EditBooking(string userId, int eventId, BookingStatus status)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var response = await _bookingRepository.UpdateBookingStatusAsync(userId, eventId, status);
            if (response.IsSucceeded)
                return Ok(response);
            return StatusCode(response.StatusCode, new { response.Message });
        }


        [HttpDelete("CancelBooking")]
        public async Task<IActionResult> CancelBooking(string userId, int eventId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var response = await _bookingRepository.CancelBookingAsync(userId, eventId);
            if (response.IsSucceeded)
                return Ok(response);
            return StatusCode(response.StatusCode, new { response.Message });
        }
        #endregion
    }
}
