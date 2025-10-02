using Chirp.Razor.Models;

using System.Linq.Expressions;

namespace Chirp.Razor.Repositories;

public interface ICheepRepository
{
    public Task<List<CheepViewModel>> ReadAll();
    public Task<List<CheepViewModel>> Read(int pageNumber, int pageSize);
    public Task<List<CheepViewModel>> Query(Expression<Func<Cheep, bool>> condition, int pageNumber, int pageSIze);
    //todo: add createcheep, updatecheep, remove cheetp osv.
}