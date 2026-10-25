using LogiCore.Domain.Cargos;
using LogiCore.Domain.Common;

namespace LogiCore.Tests;

public class RepositoryTests
{
    [Fact]
    public void Add_ThenIndexer_ReturnsSameItem()
    {
        var repository = new Repository<Cargo>();
        var cargo = TestData.Standard();

        repository.Add(cargo);

        Assert.Same(cargo, repository[cargo.Id]);
    }

    [Fact]
    public void Remove_RemovesItem()
    {
        var repository = new Repository<Cargo>();
        var cargo = TestData.Standard();
        repository.Add(cargo);

        Assert.True(repository.Remove(cargo));
        Assert.Equal(0, repository.Count);
    }

    [Fact]
    public void FindAll_ReturnsMatchingItems()
    {
        var repository = new Repository<Cargo>();
        repository.Add(TestData.Standard(10m));
        repository.Add(TestData.Standard(500m));
        repository.Add(TestData.Standard(1000m));

        var heavy = repository.FindAll(c => c.WeightKg >= 500m);

        Assert.Equal(2, heavy.Count);
    }

    [Fact]
    public void Enumeration_ReturnsItemsInOrder()
    {
        var repository = new Repository<Cargo>();
        var first = TestData.Standard();
        var second = TestData.Fragile();
        repository.Add(first);
        repository.Add(second);

        var items = new List<Cargo>();
        foreach (var item in repository)
        {
            items.Add(item);
        }

        Assert.Equal(new Cargo[] { first, second }, items);
    }

    [Fact]
    public void Indexer_MissingId_Throws()
    {
        var repository = new Repository<Cargo>();

        Assert.Throws<KeyNotFoundException>(() => repository[Guid.NewGuid()]);
    }
}
