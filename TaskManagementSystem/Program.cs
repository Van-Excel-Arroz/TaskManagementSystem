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
        private static string _dueDateStringFormat = "yyyy-MM-dd hh:mm tt";
        private static string _priorityLevels = "([1]None - [2]Low - [3]Medium - [4]High)";

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
                        currentState = mainMenuController.Run(ref _currentSelectedTodoList); break;
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
                string choice = UserInput.GetString("\nEnter: ");

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

        private static void AddTodo()
        {
            Console.WriteLine("\n=== Add Todo ===");
            string title = UserInput.GetString("Title: ", isRequired: true);
            string description = UserInput.GetString("Description: ");
            DateTime dueDate = UserInput.GetDateTime($"Due Date ({_dueDateStringFormat}): ", _dueDateStringFormat);
            PriorityLevel priorityLevel = UserInput.GetPriority($"Priority {_priorityLevels}: ", isRequired: true);
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
                int selectedTodoId = UserInput.GetInt("ID: ", isRequired: true);

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
                int selectedTodoId = UserInput.GetInt("ID: ", isRequired: true);

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

            Console.WriteLine("\nSelect the ID you want to update, you can leave some fields empty if you don't want to update it.");

            var availableTodoIds = _taskService.GetAllTodoItems(_currentSelectedTodoList!.Id).Select(t => t.Id).ToList();
            int todoId = UserInput.GetInt("ID: ", isRequired: true, availableTodoIds);
            TodoItem todo = _taskService.GetTodoItemById(todoId)!;

            todo.Title = UserInput.GetString("Title: ", isRequired: false, defaultValue: todo.Title);
            todo.Description = UserInput.GetString("Description: ", isRequired: false, defaultValue: todo.Description);
            todo.DueDate = UserInput.GetDateTime($"Due Date ({_dueDateStringFormat}): ", _dueDateStringFormat, defaultValue: todo.DueDate ?? DateTime.MinValue);
            todo.Priority = UserInput.GetPriority($"Priority {_priorityLevels}: ", isRequired: false, defaultValue: todo.Priority);

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

            _currentSelectedTodoList.Title = UserInput.GetString("New Title: ", isRequired: false, defaultValue: _currentSelectedTodoList.Title);
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

            string choice = UserInput.GetString($"Are you sure you want to delete \'{_currentSelectedTodoList.Title}\' [Y/N]: ", isRequired: true, options: new string[] { "Y", "N" });
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
    }
}
