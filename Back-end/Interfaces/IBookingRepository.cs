using EventManagmentTask.DTOs;
using EventManagmentTask.Helpers;

namespace EventManagmentTask.Interfaces
{
    public interface IBookingRepository
    {
        Task<ResponseDto> CreateBookingAsync(BookingDto dto);
        Task<ResponseDto> GetAllBookingsAsync();
        Task<ResponseDto> GetBookingByEventAsync(int eventId);
        Task<ResponseDto> GetUserBookingsAsync(string userId);
        Task<ResponseDto> UpdateBookingStatusAsync(string userId, int eventId, BookingStatus status);
        Task<ResponseDto> CancelBookingAsync(string userId, int eventId);


    }
}
