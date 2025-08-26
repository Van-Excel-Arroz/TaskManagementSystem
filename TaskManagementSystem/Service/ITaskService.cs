using TaskManagementSystem.Models;

namespace TaskManagementSystem.Service
{
    public interface ITaskService
    {
        User CreateUser(string username, string password);
        User? AuthenticateUser(string username, string password);
        IReadOnlyList<User> GetAllUsers();
        public IReadOnlyList<TodoList> GetAllTodoLists();
        void PrintUserDetails(User user);
        public void CreateTodoList(string title, int userId);
        public TodoList? GetTodoListById(int id);



    }
}
