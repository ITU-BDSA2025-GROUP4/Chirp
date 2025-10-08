
public interface IRepository<T>
{
    public Task<List<T>> ReadAll();
}