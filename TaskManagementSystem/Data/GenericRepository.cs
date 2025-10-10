using TaskManagementSystem.Models;

namespace TaskManagementSystem.Data
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class, IEntity
    {
        private readonly Dictionary<int, T> _items = new();
        private int _nextId = 1;

        public void Add(T entity)
        {
            entity.Id = _nextId++;
            _items[entity.Id] = entity;
        }

        public void Remove(int id)
        {
            if (_items.TryGetValue(id, out T? item))
            {
                _items.Remove(id);
            }
        }

        public void Update(T entity)
        {

            if (_items.TryGetValue(entity.Id, out T? existingItem))
            {
                _items[entity.Id] = entity;
            }
        }

        public T? GetById(int id)
        {
            _items.TryGetValue(id, out T? item);
            return item;
        }

        public IReadOnlyList<T> GetAll()
        {
            return _items.Values.ToList().AsReadOnly();
        }
    }
}
