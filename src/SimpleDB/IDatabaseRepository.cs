namespace SimpleDB;

public interface IDatabaseRepository<T>
{
    public IEnumerable<T> Read(int limit);
    public void Store(T record);
}
