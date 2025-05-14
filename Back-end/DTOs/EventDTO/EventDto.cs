using System.Text.Json.Serialization;

namespace EventManagmentTask.DTOs.EventDTO
{
    public class EventDto
    {
        [JsonIgnore]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string Location { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public int CategoryId { get; set; }
        public string? UserId { get; set; }

        public List<int>? TagIds { get; set; } = new List<int>();
    }
}
