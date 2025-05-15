namespace EventManagmentTask.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Event> Events { get; set; } = new List<Event>();

    }
}