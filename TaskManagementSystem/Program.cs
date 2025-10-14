using TaskManagementSystem.Controllers;
using TaskManagementSystem.Data;
using TaskManagementSystem.Models;
using TaskManagementSystem.Service;
using TaskManagementSystem.Utilities;


namespace TaskManagementSystem
{
    class Program
    {
        private static readonly ITaskService _taskService;
        private static User? _currentUser;
        private static TodoList? _currentSelectedTodoList;

        static Program()
        {
            var userRepository = new GenericRepository<User>();
            var todolistRepository = new GenericRepository<TodoList>();
            var todoItemRepository = new GenericRepository<TodoItem>();
            _taskService = new TaskService(userRepository, todolistRepository, todoItemRepository);

            DataSeeder.Initialize(_taskService);
            //var user = _taskService.AuthenticateUser("van", "lol");
            //_currentUser = user;
        }


        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Task Management System\n");
            AppState currentState = AppState.Authentication;

            var authenticationController = new AuthenticationController(_taskService);

            while (currentState != AppState.Exiting)
            {
                switch (currentState)
                {
                    case AppState.Authentication:
                        currentState = authenticationController.Run(ref _currentUser);
                        break;

                    case AppState.MainMenu:
                        if (_currentUser == null)
                        {
                            ConsoleUI.ErrorMessage("Authentication is required to access Main Menu!");
                            currentState = AppState.Authentication;
                            break;
                        }

                        var mainMenuController = new MainMenuController(_taskService, _currentUser);
                        currentState = mainMenuController.Run(ref _currentSelectedTodoList);
                        break;

                    case AppState.TodoListMenu:
                        if (_currentUser == null) currentState = AppState.Authentication;
                        if (_currentSelectedTodoList == null)
                        {
                            ConsoleUI.ErrorMessage("There is no selected todo list!");
                            currentState = AppState.MainMenu;
                            break;
                        }

                        var todoListMenuController = new TodoListMenuController(_taskService, _currentSelectedTodoList);
                        currentState = todoListMenuController.Run();
                        break;
                }
            }
        }
    }
}
