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
        private static string _dueDateFormat = "yyyy-MM-dd HH:mm aa";

        static Program()
        {
            var userRepository = new GenericRepository<User>();
            var todolistRepository = new GenericRepository<TodoList>();

            _taskService = new TaskService(userRepository, todolistRepository);
        }


        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Task Management System\n");

            while (_isProgramRunning)
            {

                while (_isAuthMenuRunning)
                {
                    DisplayAuthMenu();
                    string authMenuChoice = GetStringInput("\nEnter: ");

                    switch (authMenuChoice)
                    {
                        case "1": SignUp(); break;
                        case "2": Login(); break;
                        case "3": _isAuthMenuRunning = false; break;
                        case "4": PrintAllUsers(); break;
                        default: PrintErrorMessage(); break;
                    }
                    PauseAndClearConsole();
                }

                while (_isMainMenuRunning)
                {
                    DisplayMainMenu();
                    string mainMenuChoice = GetStringInput("\nEnter: ");

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
                        default: PrintErrorMessage(); break;
                    }
                    PauseAndClearConsole();


                    // this would run if a todolist is selected in PrintAllTodoLists() function
                    while (_isTodoListMenuRunning)
                    {
                        DisplayTodoListMenu();
                        string todoListMenuChoice = GetStringInput("\nEnter: ");

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

                int todoListId = GetIntegerInput("\nSelect a todolist: ");
                _currentSelectedTodoList = _taskService.GetTodoListById(todoListId);
                _isTodoListMenuRunning = true;
            }
        }

        private static void SignUp()
        {
            Console.WriteLine("\n=== Sign Up ===");
            string username = GetStringInput("Username: ", isRequired: true);
            string password = GetStringInput("Password: ", isRequired: true);

            _currentUser = _taskService.CreateUser(username, password);
            Console.WriteLine($"\nSuccessfully created an account!");

        }

        private static void Login()
        {
            Console.WriteLine("\n=== Login ===");
            string username = GetStringInput("Username: ", isRequired: true);
            string password = GetStringInput("Password: ", isRequired: true);

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
                PrintErrorMessage("Incorrect username or password. Please try again.");
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
        }

        private static void DisplayAuthMenu()
        {
            Console.WriteLine("[1] Sign Up");
            Console.WriteLine("[2] Log In");
            Console.WriteLine("[3] Exit");
            Console.WriteLine("[4] See all users");
        }

        private static void DisplayTodoListMenu()
        {
            Console.WriteLine($"=== Todo List \"{_currentSelectedTodoList?.Title}\" ===\n");
            Console.WriteLine("[1] Create a todo");
            Console.WriteLine("[2] Delete todo\\s");
            Console.WriteLine("[3] Rename");
            Console.WriteLine("[4] Delete");
            Console.WriteLine("[5] Back");
        }

        private static void AddTodoList()
        {
            if (_currentUser == null)
            {
                Console.WriteLine("User must be authenticated.");
                return;
            }

            Console.WriteLine("\n=== Create a TodoList ===");
            string title = GetStringInput("Title: ", isRequired: true);
            _taskService.CreateTodoList(title, _currentUser.Id);
            Console.WriteLine($"\nSuccessfully created a todolist \"{title}\".");
        }

        private static string GetStringInput(string prompt, bool isRequired = false, bool isDateTime = false, bool isPriorityLevel = false)
        {
            bool isInputPending = true;

            while (isInputPending)
            {
                Console.Write($"{prompt}");
                string userInput = Console.ReadLine() ?? string.Empty;

                if (isRequired && userInput.Length == 0)
                {
                    PrintErrorMessage("You can't leave this field empty.");
                    continue;
                }

                if (isDateTime && !DateTime.TryParse(userInput, out DateTime validDate))
                {
                    PrintErrorMessage("Invalid date format, please try again.");
                    continue;
                }

                if (isPriorityLevel && !Enum.TryParse(userInput, out PriorityLevel priority))
                {
                    PrintErrorMessage("Invalid input, please select the exact priority.");
                    continue;
                }
                isInputPending = false;
                return userInput;

            }
            return string.Empty;
        }

        private static int GetIntegerInput(string prompt)
        {
            bool isInputPending = true;

            while (isInputPending)
            {
                try
                {
                    Console.Write($"{prompt}");
                    string userInput = Console.ReadLine() ?? string.Empty;
                    int parseInput = int.Parse(userInput);
                    return parseInput;
                }
                catch (FormatException ex)
                {
                    PrintErrorMessage($"Invalid input, please try again. {ex.Message}");

                }
            }
            return -1;
        }

        private static void PrintErrorMessage(string message = "Invalid option, please try again.")
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}