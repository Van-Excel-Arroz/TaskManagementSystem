using TaskManagementSystem.Data;
using TaskManagementSystem.Models;
using TaskManagementSystem.Service;

namespace TaskManagementSystem
{
    class Program
    {
        private static readonly ITaskService _taskService;
        private static User? _currentUser;
        private static TodoList? _currentSelectedTodoList;
        private static bool _isProgramRunning = true;
        private static bool _isAuthMenuRunning = true;
        private static bool _isMainMenuRunning = false;
        private static bool _isTodoListMenuRunning = false;

        static Program()
        {
            var userRepository = new GenericRepository<User>();
            var todolistRepository = new GenericRepository<TodoList>();

            _taskService = new TaskService(userRepository, todolistRepository);
        }


        static void Main(string[] args)
        {
            Console.WriteLine("Task Management System\n");

            while (_isProgramRunning)
            {

                while (_isAuthMenuRunning)
                {
                    DisplayAuthMenu();
                    string authMenuChoice = Console.ReadLine() ?? string.Empty;

                    switch (authMenuChoice)
                    {
                        case "1": SignUp(); break;
                        case "2": Login(); break;
                        case "3": _isAuthMenuRunning = false; break;
                        case "4": PrintAllUsers(); break;
                        default: Console.WriteLine("Invalid Option, please try again."); break;
                    }
                    PauseAndClearConsole();
                }

                while (_isMainMenuRunning)
                {
                    DisplayMainMenu();
                    string mainMenuChoice = Console.ReadLine() ?? string.Empty;

                    switch (mainMenuChoice)
                    {
                        case "1": AddTodoList(); break;
                        case "2": PrintAllTodoLists(); break;
                        case "3":
                            {
                                _isMainMenuRunning = false;
                                _isAuthMenuRunning = true;
                                break;
                            }
                        default: Console.WriteLine("Invalid Option, please try again."); break;
                    }
                    PauseAndClearConsole();


                    // this would run if a todolist is selected in PrintAllTodoLists() function
                    while (_isTodoListMenuRunning)
                    {
                        DisplayTodoListMenu();
                        string todoListMenuChoice = Console.ReadLine() ?? string.Empty;

                        switch (todoListMenuChoice)
                        {
                            case "5":
                                {
                                    _isTodoListMenuRunning = false;
                                    _currentSelectedTodoList = null;
                                    break;
                                }
                        }
                        PauseAndClearConsole();
                    }

                }
            }

        }

        private static void PrintAllUsers()
        {
            var users = _taskService.GetAllUsers();
            if (!users.Any())
            {
                Console.WriteLine("No users in memory.");
            }
            else
            {
                foreach (var user in users)
                {
                    _taskService.PrintUserDetails(user);
                }
            }
        }

        private static void PrintAllTodoLists()
        {
            var todolists = _taskService.GetAllTodoLists();
            if (!todolists.Any())
            {
                Console.WriteLine("\nNo todolists in memory.");
            }
            else
            {
                foreach (var todolist in todolists)
                {
                    Console.WriteLine($"[{todolist.Id}] {todolist.Title}");
                }

                try
                {
                    Console.Write("\nSelect a todolist: ");
                    string choice = Console.ReadLine() ?? string.Empty;
                    int todoListId = int.Parse(choice);
                    _currentSelectedTodoList = _taskService.GetTodoListById(todoListId);
                    _isTodoListMenuRunning = true;
                }
                catch (FormatException ex)
                {
                    Console.WriteLine("Invalid input, please try again.", ex);
                }



            }
        }

        private static void SignUp()
        {
            Console.WriteLine("\n=== Sign Up ===");
            Console.Write("Username: ");
            string username = Console.ReadLine() ?? "";
            Console.Write("Password: ");
            string password = Console.ReadLine() ?? "";

            _currentUser = _taskService.CreateUser(username, password);
            Console.WriteLine($"\nSuccessfully created an account!");

        }

        private static void Login()
        {
            Console.WriteLine("\n=== Login ===");
            Console.Write("Username: ");
            string username = Console.ReadLine() ?? "";
            Console.Write("Password: ");
            string password = Console.ReadLine() ?? "";
            var existingUser = _taskService.AuthenticateUser(username, password);
            if (existingUser != null)
            {
                _currentUser = existingUser;
                _isAuthMenuRunning = false;
                _isMainMenuRunning = true;
                Console.WriteLine($"\nLogin Successfully, Welcome {_currentUser.Username}!");
            }
            else
            {
                Console.WriteLine("Incorrect username or password. Please try again.");
            }
        }

        private static void PauseAndClearConsole()
        {
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
            Console.Clear();
        }

        private static void DisplayMainMenu()
        {
            Console.WriteLine("[1] Create a todo list");
            Console.WriteLine("[2] Display all todo lists");
            Console.WriteLine("[3] Logout");
            Console.Write("\nEnter: ");
        }

        private static void DisplayAuthMenu()
        {
            Console.WriteLine("[1] Sign Up");
            Console.WriteLine("[2] Log In");
            Console.WriteLine("[3] Exit");
            Console.WriteLine("[4] See all users");
            Console.Write("\nEnter: ");
        }

        private static void DisplayTodoListMenu()
        {
            Console.WriteLine($"=== Todo List \"{_currentSelectedTodoList?.Title}\" ===\n");
            Console.WriteLine("[1] Create a todo");
            Console.WriteLine("[2] Delete todo\\s");
            Console.WriteLine("[3] Rename");
            Console.WriteLine("[4] Delete");
            Console.WriteLine("[5] Back");
            Console.Write("\nEnter: ");
        }

        private static void AddTodoList()
        {
            if (_currentUser == null)
            {
                Console.WriteLine("User must be authenticated.");
                return;
            }

            Console.WriteLine("\n=== Create a TodoList ===");
            Console.Write("Title: ");
            string title = Console.ReadLine() ?? string.Empty;
            _taskService.CreateTodoList(title, _currentUser.Id);
            Console.WriteLine($"\nSuccessfully created a todolist \"{title}\".");
        }
    }
}