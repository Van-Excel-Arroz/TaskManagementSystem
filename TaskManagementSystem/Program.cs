using TaskManagementSystem.Data;
using TaskManagementSystem.Models;
using TaskManagementSystem.Service;

namespace TaskManagementSystem
{
    class Program
    {
        private static readonly ITaskService _taskService;
        private static User? _currentUser;
        private static bool _isAuthMenuRunning = true;
        private static bool _isMainMenuRunning = false;

        static Program()
        {
            var userRepository = new GenericRepository<User>();

            _taskService = new TaskService(userRepository);
        }


        static void Main(string[] args)
        {
            Console.WriteLine("Task Management System\n");

            while (_isAuthMenuRunning)
            {
                Console.WriteLine("[1] Sign Up\n[2] Log In\n[3] Exit\n[4] See all users");
                Console.Write("Enter: ");
                string choice = Console.ReadLine() ?? string.Empty;

                switch (choice)
                {
                    case "1": SignUp(); break;
                    case "2": Login(); break;
                    case "3": _isAuthMenuRunning = false; break;
                    case "4": PrintAllUsers(); break;
                    default: Console.WriteLine("Invalid Option, please try again."); break;
                }
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
                Console.Clear();
            }

            while (_isMainMenuRunning)
            {
                DisplayMainMenu();
                string choice = Console.ReadLine() ?? string.Empty;

                switch (choice)
                {
                    case "2": _isMainMenuRunning = false; break;
                    default: Console.WriteLine("Invalid Option, please try again."); break;
                }
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
                Console.Clear();
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

        private static void DisplayMainMenu()
        {
            Console.WriteLine("[1] Create A todolist");
            Console.WriteLine("[2] Exit");
            Console.Write("Enter:");
        }
    }
}