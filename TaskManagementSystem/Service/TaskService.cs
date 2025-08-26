using TaskManagementSystem.Data;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.Service
{
    public class TaskService : ITaskService
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly IGenericRepository<TodoList> _todolistRepository;

        public TaskService(IGenericRepository<User> userRepository, IGenericRepository<TodoList> todolistRepository)
        {
            _userRepository = userRepository;
            _todolistRepository = todolistRepository;

        }

        public User CreateUser(string username, string password)
        {
            var newUser = new User { Username = username, Password = password };
            _userRepository.Add(newUser);
            return newUser;
        }

        public void CreateTodoList(string title, int userId)
        {
            var newTodoList = new TodoList { Title = title, UserId = userId };
            _todolistRepository.Add(newTodoList);
        }

        public User? AuthenticateUser(string username, string password)
        {
            var existingUser = _userRepository.GetAll().FirstOrDefault(u => u.Username == username && u.Password == password);
            return existingUser;
        }

        public IReadOnlyList<User> GetAllUsers()
        {
            return _userRepository.GetAll();
        }

        public IReadOnlyList<TodoList> GetAllTodoLists()
        {
            return _todolistRepository.GetAll();
        }

        public TodoList? GetTodoListById(int id)
        {
            return _todolistRepository.GetById(id);
        }

        public void PrintUserDetails(User user)
        {
            Console.WriteLine($"User ID: {user.Id}");
            Console.WriteLine($"Username: {user.Username}");
            Console.WriteLine($"Password: {user.Password}");
        }

    }
}
