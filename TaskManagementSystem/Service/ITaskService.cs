using TaskManagementSystem.Models;

namespace TaskManagementSystem.Service
{
    public interface ITaskService
    {
        User CreateUser(string username, string password);
        User? AuthenticateUser(string username, string password);
        IReadOnlyList<User> GetAllUsers();
        void PrintUserDetails(User user);



    }
}
