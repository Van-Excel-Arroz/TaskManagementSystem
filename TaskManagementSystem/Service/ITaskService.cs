using TaskManagementSystem.Models;

namespace TaskManagementSystem.Service
{
    public interface ITaskService
    {
        void CreateUser(User newUser);
        void CreateTodoList(TodoList newTodoList);
        void CreateTodoItem(TodoItem newTodo);
        void DeleteTodoItem(int id);
        User? AuthenticateUser(string username, string password);
        IReadOnlyList<User> GetAllUsers();
        IReadOnlyList<TodoList> GetAllTodoLists();
        IReadOnlyList<TodoItem> GetAllTodoItems();
        void PrintUserDetails(User user);
        TodoList? GetTodoListById(int id);



    }
}
