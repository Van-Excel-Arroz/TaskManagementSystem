using TaskManagementSystem.Models;

namespace TaskManagementSystem.Data
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class, IEntity
    {
        private readonly List<T> _items = new();
        private readonly Dictionary<int, T> _itemDictionary = new();
        private int _nextId = 1;

        public void Add(T entity)
        {
            entity.Id = _nextId++;

            _items.Add(entity);
            _itemDictionary[entity.Id] = entity;

        }

        public void Remove(int id)
        {
            if (_itemDictionary.TryGetValue(id, out T? item))
            {
                _items.Remove(item);
                _itemDictionary.Remove(id);
            }
        }

        public void Update(T entity)
        {

            if (_itemDictionary.TryGetValue(entity.Id, out T? existingItem))
            {

                var itemIndex = _items.IndexOf(existingItem);

                if (itemIndex != -1)
                {
                    _items[itemIndex] = entity;
                    _itemDictionary[entity.Id] = entity;
                }
            }
        }

        public T? GetById(int id)
        {
            _itemDictionary.TryGetValue(id, out T? item);
            return item;
        }

        public IReadOnlyList<T> GetAll()
        {
            return _items.AsReadOnly();
        }
    }
}
