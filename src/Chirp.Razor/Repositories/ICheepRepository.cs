using Chirp.Razor.Models;

using System.Linq.Expressions;

using Utils;

namespace Chirp.Razor.Repositories;

public interface ICheepRepository : IRepository<CheepDTO>
{
    public Task<List<CheepDTO>> Read(int pageNumber, int pageSize);
    public Task<List<CheepDTO>> Query(Expression<Func<Cheep, bool>> condition, int pageNumber, int pageSIze);
    //todo: add createcheep, updatecheep, remove cheetp osv.
}