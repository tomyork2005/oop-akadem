using System.Collections;

namespace LogiCore.Domain.Common;

public class Repository<T> : IReadOnlyRepository<T>, IEnumerable<T> where T : class, IEntity
{
    private readonly List<T> _items = new List<T>();

    public int Count
    {
        get { return _items.Count; }
    }

    public T this[Guid id]
    {
        get
        {
            var item = GetById(id);
            if (item == null)
            {
                throw new KeyNotFoundException($"{typeof(T).Name} с id {id} не найден");
            }
            return item;
        }
    }

    public void Add(T item)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }
        if (GetById(item.Id) != null)
        {
            throw new ArgumentException($"{typeof(T).Name} с id {item.Id} уже есть", nameof(item));
        }
        _items.Add(item);
    }

    public bool Remove(T item)
    {
        for (int i = 0; i < _items.Count; i++)
        {
            if (_items[i].Id == item.Id)
            {
                _items.RemoveAt(i);
                return true;
            }
        }
        return false;
    }

    public T? GetById(Guid id)
    {
        foreach (var item in _items)
        {
            if (item.Id == id)
            {
                return item;
            }
        }
        return null;
    }

    public IEnumerable<T> GetAll()
    {
        return this;
    }

    public IReadOnlyCollection<T> FindAll(Predicate<T> match)
    {
        var result = new List<T>();
        foreach (var item in _items)
        {
            if (match(item))
            {
                result.Add(item);
            }
        }
        return result;
    }

    public void Clear()
    {
        _items.Clear();
    }

    public IEnumerator<T> GetEnumerator()
    {
        for (int i = 0; i < _items.Count; i++)
        {
            yield return _items[i];
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
