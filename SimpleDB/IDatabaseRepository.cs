namespace SimpleDB;

public interface IDatabaseRepository<T>
{
    public IEnumerable<T> Read(int limit);
    public IEnumerable<T> Read();
    public void Store(T record);
}