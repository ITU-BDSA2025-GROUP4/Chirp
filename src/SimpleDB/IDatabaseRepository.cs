namespace SimpleDB;

public interface IDatabaseRepository<T>
{
    public IEnumerable<T> Read(int limit);
    public IEnumerable<T> ReadAll();
    public IEnumerable<T> Query(Func<T, bool> condition);
    public void Store(T record);
    public void Write();
}