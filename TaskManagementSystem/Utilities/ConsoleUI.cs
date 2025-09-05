
using TaskManagementSystem.Models;

namespace TaskManagementSystem.Utilities
{
    public class ConsoleUI
    {
        private const int IdWidth = 4;
        private const int TitleWidth = 30;
        private const int DescriptionWidth = 50;
        private const int DueDateWidth = 22;
        private const int PriorityWidth = 10;
        private const int CompletedWidth = 10;

        public static void PauseAndClearConsole()
        {
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
            Console.Clear();
        }

        public static void DisplayMainMenu()
        {
            Console.WriteLine("[1] Create a todo list");
            Console.WriteLine("[2] Display all todo lists");
            Console.WriteLine("[3] Logout");
        }

        public static void DisplayAuthMenu()
        {
            Console.WriteLine("[1] Sign Up");
            Console.WriteLine("[2] Log In");
            Console.WriteLine("[3] Exit");
            Console.WriteLine("[4] See all users");
        }

        public static void DisplayTodoListMenu(string todoListTitle)
        {
            Console.WriteLine($"=== Todo List \"{todoListTitle}\" ===\n");
            Console.WriteLine("[1] Create a todo");
            Console.WriteLine("[2] Display all todos");
            Console.WriteLine("[3] Delete todos");
            Console.WriteLine("[4] Mark todos as completed");
            Console.WriteLine("[5] Update a todo");
            Console.WriteLine("[6] Rename list");
            Console.WriteLine("[7] Delete entire list");
            Console.WriteLine("[8] Back");
        }

        public static void ErrorMessage(string message = "Invalid option, please try again.")
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void SuccessfullMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void EmptyMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void PrintTodoTableHeader()
        {
            Console.WriteLine($"\n{"ID",-IdWidth} | {"Title",-TitleWidth} | {"Description",-DescriptionWidth} | {"Due Date",-DueDateWidth} | {"Priority",-PriorityWidth} | {"Completed",-CompletedWidth}");
            Console.WriteLine($"{new string('-', IdWidth)}-+-{new string('-', TitleWidth)}-+-{new string('-', DescriptionWidth)}-+-{new string('-', DueDateWidth)}-+-{new string('-', PriorityWidth)}-+-{new string('-', CompletedWidth)}");
        }

        public static void PrintTodoRow(TodoItem todo, string dueDateStringFormat)
        {
            string dueDate = todo.DueDate?.ToString(dueDateStringFormat) ?? "N/A";
            string isCompleted = todo.IsCompleted ? "[/] Yes" : "[X] No";
            string truncatedDescription = todo.Description.Length > DescriptionWidth ? todo.Description.Substring(0, DescriptionWidth - 3) + "..." : todo.Description;

            Console.WriteLine($"{todo.Id,-IdWidth} | {todo.Title,-TitleWidth} | {truncatedDescription,-DescriptionWidth} | {dueDate,-DueDateWidth} | {todo.Priority,-PriorityWidth} | {isCompleted,-CompletedWidth} | {todo.TodoListId}");
        }
    }
}
