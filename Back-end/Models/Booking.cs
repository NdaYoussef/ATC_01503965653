using EventManagmentTask.Helpers;

namespace EventManagmentTask.Models
{
    public class Booking
    {
        public string UserId { get; set; }
        public virtual User User { get; set; }

        public int EventId { get; set; }
        public virtual Event Event { get; set; }

        public DateTime BookingDate { get; set; } = DateTime.UtcNow;
        public BookingStatus Status { get; set; } = BookingStatus.Pending;
    }
}