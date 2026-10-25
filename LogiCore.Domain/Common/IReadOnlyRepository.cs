namespace LogiCore.Domain.Common;

public interface IReadOnlyRepository<out T> where T : class, IEntity
{
    int Count { get; }

    T? GetById(Guid id);

    IEnumerable<T> GetAll();
}
