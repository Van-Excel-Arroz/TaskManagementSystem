using TaskManagementSystem.Data;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.Service
{
    public class TaskService : ITaskService
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly IGenericRepository<TodoList> _todolistRepository;
        private readonly IGenericRepository<TodoItem> _todoItemRepository;


        public TaskService(IGenericRepository<User> userRepository, IGenericRepository<TodoList> todolistRepository, IGenericRepository<TodoItem> todoItemRepository)
        {
            _userRepository = userRepository;
            _todolistRepository = todolistRepository;
            _todoItemRepository = todoItemRepository;
        }

        public void CreateUser(User newUser)
        {
            _userRepository.Add(newUser);
        }

        public void CreateTodoList(TodoList newTodoList)
        {
            _todolistRepository.Add(newTodoList);
        }

        public void CreateTodoItem(TodoItem newTodo)
        {
            _todoItemRepository.Add(newTodo);
        }

        public void DeleteTodoItem(int id)
        {
            _todoItemRepository.Remove(id);
        }

        public void DeleteTodoList(int id)
        {
            _todolistRepository.Remove(id);
            var todoIdsToDelete = GetAllTodoItems(id).Select(t => t.Id).ToList();
            foreach (int todoId in todoIdsToDelete)
            {
                _todoItemRepository.Remove(todoId);
            }
        }

        public void MarkTodoAsCompleted(TodoItem todo)
        {
            todo.IsCompleted = true;
            _todoItemRepository.Update(todo);
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

        public IEnumerable<TodoItem> GetAllTodoItems(int todoListId)
        {
            return _todoItemRepository.GetAll().Where(t => t.TodoListId == todoListId);
        }

        public TodoList? GetTodoListById(int id)
        {
            return _todolistRepository.GetById(id);
        }

        public TodoItem? GetTodoItemById(int id)
        {
            return _todoItemRepository.GetById(id);
        }
    }
}
