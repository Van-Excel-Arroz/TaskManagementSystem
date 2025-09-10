using System.Globalization;
using TaskManagementSystem.Data;
using TaskManagementSystem.Models;
using TaskManagementSystem.Service;
using TaskManagementSystem.Utilities;


namespace TaskManagementSystem
{
    class Program
    {
        private enum AppState
        {
            Authentication,
            MainMenu,
            TodoListMenu,
            Exiting
        };

        private static readonly ITaskService _taskService;
        private static User? _currentUser;
        private static TodoList? _currentSelectedTodoList;
        private static string _dueDateStringFormat = "yyyy-MM-dd hh:mm tt";
        private static string _priorityLevels = "[1]None - [2]Low - [3]Medium - [4]High)";
        private static UserInput _userInput;

        static Program()
        {
            var userRepository = new GenericRepository<User>();
            var todolistRepository = new GenericRepository<TodoList>();
            var todoItemRepository = new GenericRepository<TodoItem>();

            _taskService = new TaskService(userRepository, todolistRepository, todoItemRepository);

            UserInput userInput = new(_taskService);
            _userInput = userInput;

            DataSeeder.Initialize(_taskService);
            var user = _taskService.AuthenticateUser("van", "lol");
            _currentUser = user;
        }


        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Task Management System\n");
            AppState currentState = AppState.MainMenu;


            while (currentState != AppState.Exiting)
            {
                switch (currentState)
                {
                    case AppState.Authentication:
                        currentState = RunAuthMenu(); break;
                    case AppState.MainMenu:
                        currentState = RunMainMenu(); break;
                    case AppState.TodoListMenu:
                        currentState = RunTodoListMenu(); break;
                }
            }
        }


        private static AppState RunTodoListMenu()
        {
            while (true)
            {
                if (_currentSelectedTodoList is null)
                {
                    ConsoleUI.ErrorMessage("There is no todo list currently selected");
                    ConsoleUI.PauseAndClearConsole();

                    return AppState.MainMenu;
                }

                ConsoleUI.DisplayTodoListMenu(_currentSelectedTodoList.Title);
                string choice = _userInput.GetString("\nEnter: ");

                switch (choice)
                {
                    case "1": AddTodo(); break;
                    case "2": PrintAllTodos(out bool isTodosEmpty); break;
                    case "3": DeleteTodos(); break;
                    case "4": MarkTodosCompletion(); break;
                    case "5": UpdateTodo(); break;
                    case "6": RenameTodoListTitle(); break;
                    case "7": if (DeleteTodoList()) return AppState.MainMenu; break;
                    case "8": ConsoleUI.PauseAndClearConsole(); return AppState.MainMenu;
                    default: ConsoleUI.ErrorMessage(); break;
                }

                ConsoleUI.PauseAndClearConsole();
            }
        }

        private static AppState RunMainMenu()
        {
            while (true)
            {
                ConsoleUI.DisplayMainMenu();
                string choice = _userInput.GetString("\nEnter: ");

                switch (choice)
                {
                    case "1": AddTodoList(); break;
                    case "2": if (SetSelectedTodoList()) return AppState.TodoListMenu; break;
                    case "3": ConsoleUI.PauseAndClearConsole(); return AppState.Authentication;
                    default: ConsoleUI.ErrorMessage(); break;
                }
                ConsoleUI.PauseAndClearConsole();
            }
        }

        private static AppState RunAuthMenu()
        {
            while (true)
            {
                ConsoleUI.DisplayAuthMenu();
                string choice = _userInput.GetString("\nEnter: ");

                switch (choice)
                {
                    case "1": SignUp(); break;
                    case "2": if (Login()) return AppState.MainMenu; break;
                    case "3": return AppState.Exiting;
                    case "4": PrintAllUsers(); break;
                    default: ConsoleUI.ErrorMessage(); break;
                }
                ConsoleUI.PauseAndClearConsole();
            }
        }

        private static void PrintAllUsers()
        {
            var users = _taskService.GetAllUsers();
            if (!users.Any())
            {
                ConsoleUI.EmptyMessage("\nNo users in memory.");
            }
            else
            {
                foreach (var user in users)
                {
                    _taskService.PrintUserDetails(user);
                }
            }
        }

        private static bool SetSelectedTodoList()
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

                int todoListId = GetUserInput<int>("\nSelect a todolist: ");
                _currentSelectedTodoList = _taskService.GetTodoListById(todoListId);

                if (_currentSelectedTodoList != null)
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

        private static void PrintAllTodos(out bool isTodosEmpty)
        {
            isTodosEmpty = false;
            if (_currentSelectedTodoList is null)
            {
                ConsoleUI.ErrorMessage("There is no todo list currently selected");
                ConsoleUI.PauseAndClearConsole();
                return;
            }

            var todos = _taskService.GetAllTodoItems(_currentSelectedTodoList.Id);
            if (!todos.Any())
            {
                ConsoleUI.EmptyMessage("\nNo todos in memory.");
                isTodosEmpty = true;
            }
            else
            {
                ConsoleUI.PrintTodoTableHeader();
                foreach (var todo in todos)
                {
                    ConsoleUI.PrintTodoRow(todo, _dueDateStringFormat);
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
            ConsoleUI.SuccessfullMessage("\nSuccessfully created an account!");

        }

        private static bool Login()
        {
            Console.WriteLine("\n=== Login ===");
            string username = GetUserInput<string>("Username: ", isRequired: true);
            string password = GetUserInput<string>("Password: ", isRequired: true);

            var existingUser = _taskService.AuthenticateUser(username, password);
            if (existingUser != null)
            {
                ConsoleUI.SuccessfullMessage($"\nLogin Successfully, Welcome {existingUser.Username}!");
                ConsoleUI.PauseAndClearConsole();
                return true;
            }
            else
            {
                ConsoleUI.ErrorMessage("Incorrect username or password. Please try again.");
                return false;
            }
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
            ConsoleUI.SuccessfullMessage($"\nSuccessfully created a todolist \"{title}\"!");
        }

        private static void AddTodo()
        {
            Console.WriteLine("\n=== Add Todo ===");
            string title = GetUserInput<string>("Title: ", isRequired: true);
            string description = GetUserInput<string>("Description: ");
            DateTime dueDate = GetUserInput<DateTime>($"Due Date ({_dueDateStringFormat}): ", isDateTime: true);
            PriorityLevel priorityLevel = GetUserInput<PriorityLevel>($"Priority {_priorityLevels}: ", isRequired: true, isPriorityLevel: true);
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
            ConsoleUI.SuccessfullMessage("Successfully created a todo!");
        }

        private static void DeleteTodos()
        {
            PrintAllTodos(out bool isTodosEmpty);
            if (isTodosEmpty) return;

            bool isInputPending = true;
            List<int> selectedTodoIds = new();

            Console.WriteLine("\nSelect the IDs you want to delete, select 0 to stop.");
            while (isInputPending)
            {
                int selectedTodoId = GetUserInput<int>("ID: ", isRequired: true);

                if (selectedTodoId == 0) break;

                var todo = _taskService.GetTodoItemById(selectedTodoId);

                if (todo != null && _currentSelectedTodoList!.Id == todo.TodoListId)
                {
                    selectedTodoIds.Add(selectedTodoId);
                }
                else
                {
                    ConsoleUI.ErrorMessage($"Todo ID \'{selectedTodoId}\' does not exist.");
                }
            }
            foreach (var todoId in selectedTodoIds)
            {
                _taskService.DeleteTodoItem(todoId);
            }
            ConsoleUI.SuccessfullMessage($"Succesfully deleted todo IDs!\n");
            ConsoleUI.PauseAndClearConsole();
        }

        private static void MarkTodosCompletion()
        {
            PrintAllTodos(out bool isTodosEmpty);
            if (isTodosEmpty) return;

            bool isInputPending = true;
            List<TodoItem> selectedTodos = new();

            Console.WriteLine("\nSelect the IDs you want to mark as completed, select 0 to stop.");
            while (isInputPending)
            {
                int selectedTodoId = GetUserInput<int>("ID: ", isRequired: true);

                if (selectedTodoId == 0) break;


                var todo = _taskService.GetTodoItemById(selectedTodoId);

                if (todo != null && _currentSelectedTodoList!.Id == todo.TodoListId)
                {
                    selectedTodos.Add(todo);
                }
                else
                {
                    ConsoleUI.ErrorMessage($"Todo ID \'{selectedTodoId}\' does not exist.");
                }
            }

            foreach (var todo in selectedTodos)
            {
                _taskService.MarkTodoAsCompleted(todo);
            }

            ConsoleUI.SuccessfullMessage($"Succesfully mark as completed of the selected todo IDs!");

        }

        private static void UpdateTodo()
        {
            PrintAllTodos(out bool isTodosEmpty);
            if (isTodosEmpty) return;

            Console.WriteLine("\nSelect the ID you want to update, you can leave it empty if you don't want to update it.");
            TodoItem todo = GetUserInput<TodoItem>("ID: ", isRequired: true, isTodoId: true);

            todo.Title = GetUserInput<string>("Title: ", hasDefaultValue: true, defaultValue: todo.Title);
            todo.Description = GetUserInput<string>("Description: ", hasDefaultValue: true, defaultValue: todo.Description);
            todo.DueDate = GetUserInput<DateTime>($"Due Date ({_dueDateStringFormat}): ", hasDefaultValue: true, defaultValue: todo.DueDate ?? DateTime.MinValue, isDateTime: true);
            todo.Priority = GetUserInput<PriorityLevel>($"Priority {_priorityLevels}: ", hasDefaultValue: true, defaultValue: todo.Priority, isPriorityLevel: true);

            ConsoleUI.PrintTodoDetails(todo);


            ConsoleUI.SuccessfullMessage("Succesfully mark as completed of the selected todo IDs!");
        }


        private static void RenameTodoListTitle()
        {
            if (_currentSelectedTodoList is null)
            {
                ConsoleUI.ErrorMessage("There is no todo list currently selected");
                ConsoleUI.PauseAndClearConsole();
                return;
            }

            _currentSelectedTodoList.Title = GetUserInput<string>("New Title: ", hasDefaultValue: true, defaultValue: _currentSelectedTodoList.Title);
            ConsoleUI.SuccessfullMessage("Successfully renamed todo list!");
        }

        private static bool DeleteTodoList()
        {
            if (_currentSelectedTodoList is null)
            {
                ConsoleUI.ErrorMessage("There is no todo list currently selected");
                ConsoleUI.PauseAndClearConsole();
                return false;
            }

            string choice = GetUserInput<string>($"Are you sure you want to delete \'{_currentSelectedTodoList.Title}\' [Y/N]: ", stringOptions: new[] { "Y", "N" });
            if (choice == "Y")
            {
                ConsoleUI.SuccessfullMessage("Successfully deleted todo list!");
                _taskService.DeleteTodoList(_currentSelectedTodoList!.Id);
                return true;
            }
            else
            {
                return false;
            }
        }

        private static T GetUserInput<T>(string prompt, bool isRequired = false, bool isDateTime = false, bool isPriorityLevel = false, bool isTodoId = false, bool hasDefaultValue = false, T? defaultValue = default(T), params string[] stringOptions)
        {
            bool isInputPending = true;

            while (isInputPending)
            {
                Console.Write(prompt);
                string userInput = Console.ReadLine()?.Trim() ?? string.Empty;

                if (isRequired && userInput.Length == 0)
                {
                    ConsoleUI.ErrorMessage("You can't leave this field empty.");
                    continue;
                }

                if (hasDefaultValue && userInput == string.Empty)
                {
                    if (defaultValue != null)
                    {
                        return defaultValue;
                    }
                    else
                    {
                        ConsoleUI.ErrorMessage("No default value provided!");
                    }
                }


                if (typeof(T) == typeof(int) || typeof(T) == typeof(TodoItem))
                {
                    if (int.TryParse(userInput, out int parsedIntInput))

                    {
                        if (isTodoId)
                        {
                            var todo = _taskService.GetTodoItemById(parsedIntInput);

                            if (todo == null || _currentSelectedTodoList!.Id != todo.TodoListId)
                            {
                                ConsoleUI.ErrorMessage($"Todo ID \'{parsedIntInput}\' does not exist.");
                                continue;
                            }
                            else
                            {
                                return (T)(object)todo;
                            }
                        }
                        return (T)(object)parsedIntInput;
                    }
                    else
                    {
                        ConsoleUI.ErrorMessage("Invalid input, please only enter numbers only.");
                        continue;
                    }
                }

                if (isDateTime && (typeof(T) == typeof(DateTime)))
                {
                    if (userInput.Length == 0)
                    {
                        if (hasDefaultValue && defaultValue != null)
                        {
                            return (T)(object)defaultValue;
                        }
                        else
                        {
                            return (T)(object)DateTime.MinValue;

                        }

                    }

                    if (DateTime.TryParseExact(userInput, _dueDateStringFormat, null, DateTimeStyles.None, out DateTime validDate))
                    {
                        return (T)(object)validDate;
                    }
                    else
                    {
                        ConsoleUI.ErrorMessage("Invalid date format, please try again.");
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
                        ConsoleUI.ErrorMessage("Invalid input, please select the exact priority.");
                        continue;
                    }

                }

                if (stringOptions.Any())
                {
                    string? selectedOption = stringOptions.FirstOrDefault(opt => opt == userInput);

                    if (selectedOption != null)
                    {
                        return (T)(object)selectedOption;
                    }
                    else
                    {
                        ConsoleUI.ErrorMessage();
                        continue;
                    }

                }

                isInputPending = false;
                return (T)(object)userInput;

            }
            return (T)(object)string.Empty;
        }
    }
}
