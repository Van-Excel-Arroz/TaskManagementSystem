using TaskManagementSystem.Models;
using TaskManagementSystem.Service;
using TaskManagementSystem.Utilities;

namespace TaskManagementSystem.Controllers
{
    public class MainMenuController
    {

        private readonly ITaskService _taskService;
        private User? _currentUser;

        public MainMenuController(ITaskService taskService, User currentUser)
        {
            _taskService = taskService;
            _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser), $"{nameof(currentUser)} can not be null!");
        }

        public AppState Run(ref TodoList? currentSelectedTodoList)
        {
            while (true)
            {
                ConsoleUI.DisplayMainMenu();
                string choice = UserInput.GetString("\nEnter: ");

                switch (choice)
                {
                    case "1": AddTodoList(); break;
                    case "2": if (SetSelectedTodoList(ref currentSelectedTodoList)) return AppState.TodoListMenu; break;
                    case "3": ConsoleUI.PauseAndClearConsole(); return AppState.Authentication;
                    default: ConsoleUI.ErrorMessage(); break;
                }
                ConsoleUI.PauseAndClearConsole();
            }
        }

        public void AddTodoList()
        {
            Console.WriteLine("\n=== Create a TodoList ===");
            string title = UserInput.GetString("Title: ", isRequired: true);
            var newTodoList = new TodoList { Title = title, UserId = _currentUser.Id };
            _taskService.CreateTodoList(newTodoList);
            ConsoleUI.SuccessfullMessage($"\nSuccessfully created a todolist \"{title}\"!");
        }

        public bool SetSelectedTodoList(ref TodoList? currentSelectedTodoList)
        {
            var todolists = _taskService.GetAllTodoLists();
            if (!todolists.Any())
            {
                ConsoleUI.EmptyMessage("\nNo todolists in memory.");
                return false;
            }
            else
            {
                foreach (var todolist in todolists)
                {
                    Console.WriteLine($"[{todolist.Id}] {todolist.Title}");
                }

                var validIds = todolists.Select(t => t.Id).ToList();
                int todoListId = UserInput.GetInt("\nSelect a todolist: ", isRequired: false, validIds);
                currentSelectedTodoList = _taskService.GetTodoListById(todoListId);

                if (currentSelectedTodoList != null)
                {
                    ConsoleUI.PauseAndClearConsole();
                    return true;
                }
                else
                {
                    ConsoleUI.ErrorMessage();
                    return false;
                }

            }
        }
    }
}
