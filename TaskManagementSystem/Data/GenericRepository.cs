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
            if (DoesIdExist(id))
            {
                var item = _itemDictionary[id];
                _items.Remove(item);
                _itemDictionary.Remove(id);
            }
        }

        public void Update(T entity)
        {

            if (DoesIdExist(entity.Id))
            {
                var itemIndex = _items.FindIndex(item => item.Id == entity.Id);

                if (itemIndex != -1)
                {
                    _items[itemIndex] = entity;
                    _itemDictionary[entity.Id] = entity;
                }
            }
        }

        public T? GetById(int id)
        {

            if (DoesIdExist(id))
            {
                return _itemDictionary[id];
            }
            return null;
        }

        public IReadOnlyList<T> GetAll()
        {
            return _items.AsReadOnly();
        }

        public bool DoesIdExist(int id)
        {
            return _itemDictionary.ContainsKey(id);
        }
    }
}
