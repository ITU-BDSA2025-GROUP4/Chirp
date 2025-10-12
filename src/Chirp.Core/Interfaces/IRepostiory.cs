namespace Chirp.Core.Interfaces;

public interface IRepository<T>
{
    public Task<List<T>> ReadAll();
}