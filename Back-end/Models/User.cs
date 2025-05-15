using EventManagmentTask.Helpers;
using Microsoft.AspNetCore.Identity;

namespace EventManagmentTask.Models
{
    public class User :IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Role Role { get; set; }
        public DateTime RegisterdAt { get; set; } = DateTime.UtcNow;
        public bool IsConfirmed { get; set; } = false;

        public virtual ICollection<Event>? Events { get; set; } = new List<Event>();
        public virtual ICollection<Booking>? Bookings { get; set; } = new List<Booking>();
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    }
}
