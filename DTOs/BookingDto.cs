using EventManagmentTask.Helpers;
using EventManagmentTask.Models;

namespace EventManagmentTask.DTOs
{
    public class BookingDto
    {
        public DateTime BookingDate { get; set; } = DateTime.UtcNow;
        public BookingStatus BookingStatus { get; set; } = BookingStatus.Pending;

        public string UserId { get; set; }
        public int EventId { get; set; }
        public string EventTitle { get; set; }
    }
}
