using TaskManagementSystem.Models;

namespace TaskManagementSystem.Data
{
    public interface IGenericRepository<T> where T : class, IEntity
    {
        public void Add(T entity);

        public void Remove(int id);

        public void Update(T entity);

        public T? GetById(int id);

        public IReadOnlyList<T> GetAll();

        public bool DoesIdExist(int id);

    }
}
