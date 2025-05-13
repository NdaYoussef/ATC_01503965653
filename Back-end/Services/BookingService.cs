using EventManagmentTask.Data;
using EventManagmentTask.DTOs;
using EventManagmentTask.Helpers;
using EventManagmentTask.Interfaces;
using EventManagmentTask.Models;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace EventManagmentTask.Services
{
    public class BookingService : IBookingRepository
    {
        private readonly EventManagmentDbContext _context;

        #region Constructor
        public BookingService(EventManagmentDbContext context)
        {
            _context = context;
        }
        #endregion

        #region Services
   

        public async Task<ResponseDto> CreateBookingAsync(BookingDto dto)
        {
            if (await _context.Bookings.AnyAsync(b => b.UserId == dto.UserId && b.EventId == dto.EventId))
            {
                return new ResponseDto
                {
                    IsSucceeded = false,
                    Message = "User already booked this event.",
                    StatusCode = (int)HttpStatusCode.Conflict
                };
            }

            var booking = dto.Adapt<Booking>();
            booking.Status = BookingStatus.Pending;
            booking.BookingDate = DateTime.UtcNow;

            await _context.Bookings.AddAsync(booking);
            await _context.SaveChangesAsync();

            return new ResponseDto
            {
                IsSucceeded = true,
                Message = "Booking created successfully.",
                StatusCode = (int)HttpStatusCode.Created,
                Data = booking.Adapt<BookingDto>()
            };
        }

        public async Task<ResponseDto> GetAllBookingsAsync()
        {
            var bookings = await _context.Bookings
             .Include(b => b.User)
             .Include(b => b.Event)
             .ToListAsync();

            var bookingDtos = bookings.Select(b => new BookingDto
            {
                UserId = b.UserId,
                EventId = b.EventId,
                EventTitle = b.Event.Title,
                BookingDate = b.BookingDate,
                BookingStatus = b.Status
            }).ToList();

            return new ResponseDto
            {
                IsSucceeded = true,
                StatusCode = 200,
                Data = bookingDtos
            };
        }

        public async Task<ResponseDto> GetBookingByEventAsync(int eventId)
        {
            var bookings = await _context.Bookings
            .Where(b => b.EventId == eventId)
            .Include(b => b.User)
            .Include(b => b.Event)
            .ToListAsync();

            if (bookings.Count == 0)
            {
                return new ResponseDto
                {
                    IsSucceeded = false,
                    Message = "No bookings found for this event.",
                    StatusCode = 404
                };
            }

            var bookingDtos = bookings.Select(b => new BookingDto
            {
                UserId = b.UserId,
                EventId = b.EventId
            }).ToList();

            return new ResponseDto
            {
                IsSucceeded = true,
                StatusCode = 200,
                Data = bookingDtos
            };
        }

        public async Task<ResponseDto> GetUserBookingsAsync(string userId)
        {
            var bookings = await _context.Bookings
                                                .Include(b => b.Event)
                                                .Include(b => b.User)
                                                .Where(b => b.UserId == userId)
                                                .ToListAsync();

            if (bookings.Count == 0)
            {
                return new ResponseDto
                {
                    IsSucceeded = false,
                    Message = "No bookings found for this user.",
                    StatusCode = 404
                };
            }

            var bookingDtos = bookings.Select(b => new BookingDto
            {
                UserId = b.UserId,
                EventId = b.EventId,
                BookingDate = b.BookingDate,
                BookingStatus = b.Status
            }).ToList();

            return new ResponseDto
            {
                IsSucceeded = true,
                StatusCode = 200,
                Data = bookingDtos
            };
        }

        public async Task<ResponseDto> UpdateBookingStatusAsync(string userId, int eventId, BookingStatus newStatus)
        {
            var booking = await _context.Bookings
            .FirstOrDefaultAsync(b => b.UserId == userId && b.EventId == eventId);

            if (booking == null)
            {
                return new ResponseDto
                {
                    IsSucceeded = false,
                    Message = "Booking not found.",
                    StatusCode = 404
                };
            }

            newStatus = BookingStatus.Confirmed;
            await _context.SaveChangesAsync();

            return new ResponseDto
            {
                IsSucceeded = true,
                Message = "Booking status updated.",
                StatusCode = 200,
                Data = booking.Adapt<BookingDto>()
            };
        }

        public async Task<ResponseDto> CancelBookingAsync(string userId, int eventId)
        {
            var booking = await _context.Bookings
             .FirstOrDefaultAsync(b => b.UserId == userId && b.EventId == eventId);

            if (booking == null)
            {
                return new ResponseDto
                {
                    IsSucceeded = false,
                    Message = "Booking not found.",
                    StatusCode = 404
                };
            }

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();

            return new ResponseDto
            {
                IsSucceeded = true,
                Message = "Booking deleted.",
                StatusCode = 200
            };
        }
        #endregion

    }
}
