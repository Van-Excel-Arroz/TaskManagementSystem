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
            var todoItemRepository = new GenericRepository<TodoItem>();

            _taskService = new TaskService(userRepository, todolistRepository, todoItemRepository);
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
                    string authMenuChoice = GetUserInput<string>("\nEnter: ");

                    switch (authMenuChoice)
                    {
                        case "1": SignUp(); break;
                        case "2": Login(); break;
                        case "3": _isAuthMenuRunning = false; break;
                        case "4": PrintAllUsers(); break;
                        default: ErrorMessage(); break;
                    }
                    PauseAndClearConsole();
                }

                while (_isMainMenuRunning)
                {
                    DisplayMainMenu();
                    string mainMenuChoice = GetUserInput<string>("\nEnter: ");

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
                        default: ErrorMessage(); break;
                    }
                    PauseAndClearConsole();


                    // this would run if a todolist is selected in PrintAllTodoLists() function
                    while (_isTodoListMenuRunning)
                    {
                        DisplayTodoListMenu();
                        string todoListMenuChoice = GetUserInput<string>("\nEnter: ");

                        switch (todoListMenuChoice)
                        {
                            case "1": AddTodo(); break;
                            case "2": break;
                            case "3": break;
                            case "4": break;
                            case "5": break;
                            case "6": break;
                            case "7": break;
                            case "8":
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
                EmptyMessage("\nNo users in memory.");
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
                EmptyMessage("\nNo todolists in memory.");
            }
            else
            {
                foreach (var todolist in todolists)
                {
                    Console.WriteLine($"[{todolist.Id}] {todolist.Title}");
                }

                int todoListId = GetUserInput<int>("\nSelect a todolist: ");
                _currentSelectedTodoList = _taskService.GetTodoListById(todoListId);
                _isTodoListMenuRunning = true;
            }
        }

        private static void SignUp()
        {
            Console.WriteLine("\n=== Sign Up ===");
            string username = GetUserInput<string>("Username: ", isRequired: true);
            string password = GetUserInput<string>("Password: ", isRequired: true);

            _currentUser = new User { Username = username, Password = password };
            _taskService.CreateUser(_currentUser);
            SuccessfullMessage("\nSuccessfully created an account!");

        }

        private static void Login()
        {
            Console.WriteLine("\n=== Login ===");
            string username = GetUserInput<string>("Username: ", isRequired: true);
            string password = GetUserInput<string>("Password: ", isRequired: true);

            var existingUser = _taskService.AuthenticateUser(username, password);
            if (existingUser != null)
            {
                _currentUser = existingUser;
                _isAuthMenuRunning = false;
                _isMainMenuRunning = true;
                SuccessfullMessage($"\nLogin Successfully, Welcome {_currentUser.Username}!");

            }
            else
            {
                ErrorMessage("Incorrect username or password. Please try again.");
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
            Console.WriteLine("[2] Display all todos");
            Console.WriteLine("[3] Delete todos");
            Console.WriteLine("[4] Mark todos as completed");
            Console.WriteLine("[5] Update a todo");
            Console.WriteLine("[6] Rename list");
            Console.WriteLine("[7] Delete entire list");
            Console.WriteLine("[8] Back");
        }

        private static void AddTodoList()
        {
            if (_currentUser == null)
            {
                Console.WriteLine("User must be authenticated.");
                return;
            }

            Console.WriteLine("\n=== Create a TodoList ===");
            string title = GetUserInput<string>("Title: ", isRequired: true);
            var newTodoList = new TodoList { Title = title, UserId = _currentUser.Id };
            _taskService.CreateTodoList(newTodoList);
            SuccessfullMessage($"\nSuccessfully created a todolist \"{title}\"!");
        }

        private static void AddTodo()
        {
            Console.WriteLine("\n=== Add Todo ===");
            string title = GetUserInput<string>("Title: ", isRequired: true);
            string description = GetUserInput<string>("Description: ");
            string dueDate = GetUserInput<string>($"Due Date ({_dueDateFormat}): ", isDateTime: true);
            string priorityLevel = GetUserInput<string>("Priority (None, Low, Medium, High): ", isRequired: true, isPriorityLevel: true);

            DateTime? parsedDueDate;

            if (string.IsNullOrEmpty(dueDate))
            {
                parsedDueDate = null;
            }
            else
            {
                parsedDueDate = DateTime.ParseExact(dueDate, _dueDateFormat, null);
            }

            var parsedPriorityLevel = Enum.Parse<PriorityLevel>(priorityLevel);

            var newTodo = new TodoItem { Title = title, Description = description, DueDate = parsedDueDate, Priority = parsedPriorityLevel, TodoListId = _currentSelectedTodoList!.Id };
            _taskService.CreateTodoItem(newTodo);
            SuccessfullMessage("Successfully created a todo!");
        }

        private static T GetUserInput<T>(string prompt, bool isRequired = false, bool isDateTime = false, bool isPriorityLevel = false)
        {
            bool isInputPending = true;

            while (isInputPending)
            {
                Console.Write(prompt);
                string userInput = Console.ReadLine() ?? string.Empty;

                if (isRequired && userInput.Length == 0)
                {
                    ErrorMessage("You can't leave this field empty.");
                    continue;
                }

                if (typeof(T) == typeof(int))
                {
                    if (int.TryParse(userInput, out int parsedInt))
                    {
                        return (T)(object)parsedInt;
                    }
                    else
                    {
                        ErrorMessage("Invalid input, please only enter numbers only.");
                        continue;
                    }
                }

                if (isDateTime && userInput.Length > 0 && (typeof(T) == typeof(DateTime)))
                {
                    if (DateTime.TryParse(userInput, out DateTime validDate))
                    {
                        return (T)(object)validDate;
                    }
                    else
                    {
                        ErrorMessage("Invalid date format, please try again.");
                        continue;
                    }

                }

                if (isPriorityLevel && (typeof(T) == typeof(PriorityLevel)))
                {
                    if (Enum.TryParse(userInput, out PriorityLevel priority))
                    {
                        return (T)(object)priority;
                    }
                    {
                        ErrorMessage("Invalid input, please select the exact priority.");
                        continue;
                    }

                }
                isInputPending = false;
                return (T)(object)userInput;

            }
            return (T)(object)string.Empty;
        }


        private static void ErrorMessage(string message = "Invalid option, please try again.")
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        private static void SuccessfullMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        private static void EmptyMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}