using System.Linq.Expressions;

using Chirp.Core.Entities;

namespace Chirp.Core.Interfaces;

public interface ICheepRepository : IRepository<CheepDTO>
{
    public Task<bool> Add(CheepDTO cheepDTO);
    public Task<List<CheepDTO>> Read(int pageNumber, int pageSize);
    public Task<List<CheepDTO>> Query(Expression<Func<Cheep, bool>> condition, int pageNumber, int pageSIze);
}