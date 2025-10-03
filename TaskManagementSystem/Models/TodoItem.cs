


namespace TaskManagementSystem.Models
{
    public enum PriorityLevel
    {
        None = 1,
        Low = 2,
        Medium = 3,
        High = 4
    }

    public class TodoItem : IEntity
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public bool IsCompleted { get; set; }

        public DateTime? DueDate { get; set; }

        public PriorityLevel Priority { get; set; }

        public int TodoListId { get; set; }
    }
}
