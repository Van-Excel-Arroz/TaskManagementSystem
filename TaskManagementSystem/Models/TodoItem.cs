


namespace TaskManagementSystem.Models
{
    public enum PriorityLevel
    {
        None,
        Low,
        Medium,
        High
    }

    public class TodoItem : IEntity
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public bool IsCompleted { get; set; }

        public DateTime? DueDate { get; set; }

        public PriorityLevel Priority { get; set; }

        public int TodoListId { get; set; }
    }
}
