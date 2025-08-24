namespace TaskManagementSystem.Models
{
    public class TodoList : IEntity
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public int UserId { get; set; }
    }
}
