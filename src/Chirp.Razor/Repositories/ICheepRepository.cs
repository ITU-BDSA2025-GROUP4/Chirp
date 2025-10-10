using Chirp.Razor.Models;

using System.Linq.Expressions;

namespace Chirp.Razor.Repositories;

public interface ICheepRepository
{
    public Task<List<CheepViewModel>> ReadAll();
    public Task<List<CheepViewModel>> Read(int pageNumber, int pageSize);
    public Task<List<CheepViewModel>> Query(Expression<Func<Cheep, bool>> condition, int pageNumber, int pageSIze);
    public Task<CheepViewModel> Create(Cheep cheep);
    public Task<CheepViewModel> Update(Cheep cheep);
    public Task<CheepViewModel> Delete(Cheep cheep);
}