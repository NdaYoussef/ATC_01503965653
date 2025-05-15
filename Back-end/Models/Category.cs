namespace EventManagmentTask.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Event>? Events { get; set; } = new List<Event>();

    }
}