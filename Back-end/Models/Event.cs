namespace EventManagmentTask.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string Location { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public string UserId { get; set; }  // FK to User
        public virtual User User { get; set; }

        public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    }
}
