using TaskManagementSystem.Data;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.Service
{
    public class TaskService : ITaskService
    {
        private readonly IGenericRepository<User> _userRepository;

        public TaskService(IGenericRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }

        public User CreateUser(string username, string password)
        {
            var newUser = new User { Username = username, Password = password };
            _userRepository.Add(newUser);
            return newUser;
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

        public void PrintUserDetails(User user)
        {
            Console.WriteLine($"User ID: {user.Id}");
            Console.WriteLine($"Username: {user.Username}");
            Console.WriteLine($"Password: {user.Password}");
        }

    }
}
