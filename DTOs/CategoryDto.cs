namespace EventManagmentTask.DTOs
{
    public class CategoryDto
    {
        public string Name { get; set; }

        public ICollection<EventDto>? Events { get; set; } = new List<EventDto>();
    }
}
