using TaskManagementSystem.Models;

namespace TaskManagementSystem.Service
{
    public interface ITaskService
    {
        void CreateUser(User newUser);
        void CreateTodoList(TodoList newTodoList);
        void CreateTodoItem(TodoItem newTodo);
        void DeleteTodoItem(int id);
        void DeleteTodoList(int id);
        void MarkTodoAsCompleted(TodoItem todo);
        User? AuthenticateUser(string username, string password);
        IReadOnlyList<User> GetAllUsers();
        IReadOnlyList<TodoList> GetAllTodoLists();
        IEnumerable<TodoItem> GetAllTodoItems(int todoListId);
        void PrintUserDetails(User user);
        TodoList? GetTodoListById(int id);
        TodoItem? GetTodoItemById(int id);



    }
}
