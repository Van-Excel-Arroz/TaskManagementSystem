using TaskManagementSystem.Models;
using TaskManagementSystem.Service;
using TaskManagementSystem.Utilities;

namespace TaskManagementSystem.Controllers
{
    class AuthenticationController
    {
        private readonly ITaskService _taskService;
        private User? _currentUser;

        public AuthenticationController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        public AppState Run(ref User? currentUser)
        {
            while (true)
            {
                ConsoleUI.DisplayAuthMenu();
                string choice = UserInput.GetString("\nEnter: ");

                switch (choice)
                {
                    case "1": SignUp(); break;
                    case "2":
                        if (Login())
                        {
                            currentUser = _currentUser;
                            return AppState.MainMenu;
                        }
                        break;
                    case "3": return AppState.Exiting;
                    case "4": PrintAllUsers(); break;
                    default: ConsoleUI.ErrorMessage(); break;
                }
                ConsoleUI.PauseAndClearConsole();
            }
        }

        public void SignUp()
        {
            Console.WriteLine("\n=== Sign Up ===");
            string username = UserInput.GetString("Username: ", isRequired: true);
            string password = UserInput.GetString("Password: ", isRequired: true);

            var newUser = new User { Username = username, Password = password };
            _currentUser = newUser;
            _taskService.CreateUser(newUser);
            ConsoleUI.SuccessfullMessage("\nSuccessfully created an account!");

        }

        public bool Login()
        {
            Console.WriteLine("\n=== Login ===");
            string username = UserInput.GetString("Username: ", isRequired: true);
            string password = UserInput.GetString("Password: ", isRequired: true);

            var existingUser = _taskService.AuthenticateUser(username, password);
            if (existingUser != null)
            {
                ConsoleUI.SuccessfullMessage($"\nLogin Successfully, Welcome {existingUser.Username}!");
                ConsoleUI.PauseAndClearConsole();
                _currentUser = existingUser;
                return true;
            }
            else
            {
                ConsoleUI.ErrorMessage("Incorrect username or password. Please try again.");
                return false;
            }
        }


        public void PrintAllUsers()
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
                    ConsoleUI.PrintUserDetails(user);
                }
            }
        }

    }
}
