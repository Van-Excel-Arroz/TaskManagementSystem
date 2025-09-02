using System.Globalization;
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
        private static bool _isAuthMenuRunning = false;
        private static bool _isMainMenuRunning = true;
        private static bool _isTodoListMenuRunning = false;
        private static string _dueDateFormat = "yyyy-MM-dd hh:mm tt";

        private const int IdWidth = 4;
        private const int TitleWidth = 30;
        private const int DescriptionWidth = 50;
        private const int DueDateWidth = 22;
        private const int PriorityWidth = 10;
        private const int CompletedWidth = 10;

        static Program()
        {
            var userRepository = new GenericRepository<User>();
            var todolistRepository = new GenericRepository<TodoList>();
            var todoItemRepository = new GenericRepository<TodoItem>();

            _taskService = new TaskService(userRepository, todolistRepository, todoItemRepository);
            SeedData();
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
                            case "2": PrintAllTodos(); break;
                            case "3": DeleteTodos(); break;
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

        private static void PrintAllTodos()
        {
            var todos = _taskService.GetAllTodoItems(_currentSelectedTodoList!.Id);
            if (!todos.Any())
            {
                EmptyMessage("\nNo todos in memory.");
            }
            else
            {
                PrintTodoTableHeader();

                foreach (var todo in todos)
                {

                    PrintTodoRow(todo);

                }
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
            DateTime dueDate = GetUserInput<DateTime>($"Due Date ({_dueDateFormat}): ", isDateTime: true);
            PriorityLevel priorityLevel = GetUserInput<PriorityLevel>("Priority (None, Low, Medium, High): ", isRequired: true, isPriorityLevel: true);
            DateTime? parsedDueDate = dueDate == DateTime.MinValue ? null : dueDate;

            var newTodo = new TodoItem
            {
                Title = title,
                Description = description,
                IsCompleted = false,
                DueDate = parsedDueDate,
                Priority = priorityLevel,
                TodoListId = _currentSelectedTodoList!.Id
            };
            _taskService.CreateTodoItem(newTodo);
            SuccessfullMessage("Successfully created a todo!");
        }

        private static void DeleteTodos()
        {
            PrintAllTodos();

            bool isInputPending = true;
            List<int> selectedTodoIds = new();

            Console.WriteLine("\nSelect the IDs you want to delete, select 0 to stop.");
            while (isInputPending)
            {
                int selectedTodoId = GetUserInput<int>("ID: ", isRequired: true);

                if (selectedTodoId == 0)
                {
                    isInputPending = false;
                    break;
                }

                var todo = _taskService.GetTodoItemById(selectedTodoId);

                if (todo != null && _currentSelectedTodoList!.Id == todo.TodoListId)
                {
                    selectedTodoIds.Add(selectedTodoId);
                }
                else
                {
                    ErrorMessage($"Todo ID \'{selectedTodoId}\' does not exist.");
                }
            }

            foreach (var todoId in selectedTodoIds)
            {
                _taskService.DeleteTodoItem(todoId);
            }

            SuccessfullMessage($"Succesfully deleted todo IDs!\n");
            PauseAndClearConsole();
            PrintAllTodos();
            // this will print the updated list of todos after deletion

        }

        private static T GetUserInput<T>(string prompt, bool isRequired = false, bool isDateTime = false, bool isPriorityLevel = false)
        {
            bool isInputPending = true;

            while (isInputPending)
            {
                Console.Write(prompt);
                string userInput = Console.ReadLine()?.Trim() ?? string.Empty;

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

                if (isDateTime && (typeof(T) == typeof(DateTime)))
                {
                    if (userInput.Length == 0)
                    {
                        return (T)(object)DateTime.MinValue;
                    }
                    if (DateTime.TryParseExact(userInput, _dueDateFormat, null, DateTimeStyles.None, out DateTime validDate))
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

        private static void PrintTodoTableHeader()
        {
            Console.WriteLine($"\n{"ID",-IdWidth} | {"Title",-TitleWidth} | {"Description",-DescriptionWidth} | {"Due Date",-DueDateWidth} | {"Priority",-PriorityWidth} | {"Completed",-CompletedWidth}");
            Console.WriteLine($"{new string('-', IdWidth)}-+-{new string('-', TitleWidth)}-+-{new string('-', DescriptionWidth)}-+-{new string('-', DueDateWidth)}-+-{new string('-', PriorityWidth)}-+-{new string('-', CompletedWidth)}");
        }

        private static void PrintTodoRow(TodoItem todo)
        {
            string dueDate = todo.DueDate?.ToString(_dueDateFormat) ?? "N/A";
            string isCompleted = todo.IsCompleted ? "[/] Yes" : "[X] No";
            string truncatedDescription = todo.Description.Length > DescriptionWidth ? todo.Description.Substring(0, DescriptionWidth - 3) + "..." : todo.Description;

            Console.WriteLine($"{todo.Id,-IdWidth} | {todo.Title,-TitleWidth} | {truncatedDescription,-DescriptionWidth} | {dueDate,-DueDateWidth} | {todo.Priority,-PriorityWidth} | {isCompleted,-CompletedWidth} | {todo.TodoListId}");
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

        private static void SeedData()
        {
            var tempUser = new User { Username = "van", Password = "lol" };
            _taskService.CreateUser(tempUser);
            _currentUser = tempUser;


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
                _taskService.CreateTodoList(todolist);
            }

            foreach (var todo in todoItems)
            {
                _taskService.CreateTodoItem(todo);
            }
        }
    }
}
