using System.Linq.Expressions;

using Chirp.Core.Entities;

namespace Chirp.Core.Interfaces;

public interface ICheepRepository
{
    public Task<List<CheepDTO>> ReadAll();
    public Task<List<CheepDTO>> Read(int pageNumber, int pageSize);
    public Task<List<CheepDTO>> Query(Expression<Func<Cheep, bool>> condition, int pageNumber, int pageSIze);
}