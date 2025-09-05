using TaskManagementSystem.Models;
using TaskManagementSystem.Service;

namespace TaskManagementSystem.Utilities
{
    public class DataSeeder
    {
        public static void Initialize(ITaskService taskService)
        {
            var tempUser = new User { Username = "van", Password = "lol" };
            taskService.CreateUser(tempUser);

            var todoItems = new List<TodoItem>
            {
                new TodoItem
                {
                    Id = 1,
                    Title = "Finalize Q3 Report",
                    Description = "Review the final draft, add the latest sales figures, and send to management.",
                    IsCompleted = false,
                    DueDate = DateTime.Now.AddDays(3),
                    Priority = PriorityLevel.High,
                    TodoListId = 1
                },
                new TodoItem
                {
                    Id = 2,
                    Title = "Prepare for Client Meeting",
                    Description = "Create a presentation deck and gather necessary documents for the Acme Corp meeting.",
                    IsCompleted = false,
                    DueDate = DateTime.Now.AddDays(7),
                    Priority = PriorityLevel.Medium,
                    TodoListId = 1
                },
                new TodoItem
                {
                    Id = 3,
                    Title = "Code Review for Feature X",
                    Description = "Reviewed pull request #123 and provided feedback.",
                    IsCompleted = true,
                    DueDate = DateTime.Now.AddDays(-2),
                    Priority = PriorityLevel.Medium,
                    TodoListId = 1
                },
                new TodoItem
                {
                    Id = 4,
                    Title = "Organize Digital Files",
                    Description = "Clean up the project folder on the shared drive.",
                    IsCompleted = false,
                    DueDate = null,
                    Priority = PriorityLevel.Low,
                    TodoListId = 1
                },

                new TodoItem
                {
                    Id = 5,
                    Title = "Pay electricity bill",
                    Description = "Due by the end of the week.",
                    IsCompleted = false,
                    DueDate = DateTime.Now.AddDays(2),
                    Priority = PriorityLevel.High,
                    TodoListId = 2
                },
                new TodoItem
                {
                    Id = 6,
                    Title = "Call Mom",
                    Description = "",
                    IsCompleted = false,
                    DueDate = null,
                    Priority = PriorityLevel.None,
                    TodoListId = 2
                },
                new TodoItem
                {
                    Id = 7,
                    Title = "Mow the lawn",
                    Description = "The grass is getting long.",
                    IsCompleted = true,
                    DueDate = DateTime.Now.AddDays(-1),
                    Priority = PriorityLevel.Low,
                    TodoListId = 2
                },
                new TodoItem
                {
                    Id = 8,
                    Title = "Dentist Appointment",
                    Description = "Annual check-up and cleaning.",
                    IsCompleted = false,
                    DueDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 28, 14, 30, 0), // Specific time
                    Priority = PriorityLevel.Medium,
                    TodoListId = 2
                },

                new TodoItem
                {
                    Id = 9,
                    Title = "Milk",
                    Description = "1 gallon, 2%",
                    IsCompleted = false,
                    DueDate = null,
                    Priority = PriorityLevel.None,
                    TodoListId = 3
                },
                new TodoItem
                {
                    Id = 10,
                    Title = "Bread",
                    Description = "Whole wheat",
                    IsCompleted = true,
                    DueDate = null,
                    Priority = PriorityLevel.None,
                    TodoListId = 3
                },
                new TodoItem
                {
                    Id = 11,
                    Title = "Chicken Breasts",
                    Description = "",
                    IsCompleted = false,
                    DueDate = null,
                    Priority = PriorityLevel.None,
                    TodoListId = 3
                },
                new TodoItem
                {
                    Id = 12,
                    Title = "Apples",
                    Description = "Fuji or Gala",
                    IsCompleted = false,
                    DueDate = null,
                    Priority = PriorityLevel.None,
                    TodoListId = 3
                },
                new TodoItem
                {
                    Id = 1,
                    Title = "Finalize Q4 Presentation",
                    Description = "Review slides and add final metrics.",
                    IsCompleted = false,
                    DueDate = DateTime.Now.Date.AddDays(3).AddHours(16), // e.g., "2023-10-29 04:00 PM"
                    Priority = PriorityLevel.High,
                    TodoListId = 1
                },
                new TodoItem
                {
                    Id = 2,
                    Title = "Submit Expense Report",
                    Description = "For last month's travel.",
                    IsCompleted = false,
                    DueDate = DateTime.Now.Date.AddDays(1).AddHours(11).AddMinutes(30), // e.g., "2023-10-27 11:30 AM"
                    Priority = PriorityLevel.Medium,
                    TodoListId = 1
                },
                new TodoItem
                {
                    Id = 3,
                    Title = "Deploy API v1.2",
                    Description = "Deployed successfully last week.",
                    IsCompleted = true,
                    DueDate = DateTime.Now.Date.AddDays(-5).AddHours(15), // e.g., "2023-10-21 03:00 PM"
                    Priority = PriorityLevel.High,
                    TodoListId = 1
                },

                // --- Items for List 2: Home & Personal ---
                new TodoItem
                {
                    Id = 4,
                    Title = "Pay credit card bill",
                    Description = "Due by the end of the month.",
                    IsCompleted = false,
                    DueDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 28, 23, 59, 0), // e.g., "2023-10-28 11:59 PM"
                    Priority = PriorityLevel.High,
                    TodoListId = 2
                },
                new TodoItem
                {
                    Id = 5,
                    Title = "Schedule car maintenance",
                    Description = "Oil change and tire rotation.",
                    IsCompleted = false,
                    DueDate = null,
                    Priority = PriorityLevel.Low,
                    TodoListId = 2
                },
                new TodoItem
                {
                    Id = 6,
                    Title = "Return library books",
                    Description = "The 'Dune' series.",
                    IsCompleted = true,
                    DueDate = DateTime.Now.Date.AddDays(-2).AddHours(13), // e.g., "2023-10-24 01:00 PM"
                    Priority = PriorityLevel.None,
                    TodoListId = 2
                },

                // --- Items for List 3: Health & Fitness ---
                new TodoItem
                {
                    Id = 7,
                    Title = "Morning Run",
                    Description = "5k run in the park.",
                    IsCompleted = false,
                    DueDate = DateTime.Now.Date.AddDays(1).AddHours(7), // e.g., "2023-10-27 07:00 AM"
                    Priority = PriorityLevel.Medium,
                    TodoListId = 3
                },
                new TodoItem
                {
                    Id = 8,
                    Title = "Meal Prep for the Week",
                    Description = "Chicken, rice, and vegetables.",
                    IsCompleted = false,
                    DueDate = DateTime.Now.Date.AddDays(2).AddHours(18).AddMinutes(30), // e.g., "2023-10-28 06:30 PM"
                    Priority = PriorityLevel.Medium,
                    TodoListId = 3
                },
                new TodoItem
                {
                    Id = 9,
                    Title = "Order more protein powder",
                    Description = "",
                    IsCompleted = false,
                    DueDate = null,
                    Priority = PriorityLevel.Low,
                    TodoListId = 3
                }
            };

            var todoLists = new List<TodoList>
            {
                new TodoList { Id = 1, Title = "Work Projects", UserId = 1 },
                new TodoList { Id = 2, Title = "Home & Personal", UserId = 1 },
                new TodoList { Id = 3, Title = "Health & Fitness", UserId = 1 }
            };

            foreach (var todolist in todoLists)
            {
                taskService.CreateTodoList(todolist);
            }

            foreach (var todo in todoItems)
            {
                taskService.CreateTodoItem(todo);
            }
        }
    }
}
