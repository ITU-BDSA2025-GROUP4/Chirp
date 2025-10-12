using Chirp.Razor.Models;

using System.Linq.Expressions;

using Chirp.Razor.Application.Contracts;

namespace Chirp.Razor.Repositories;

public interface ICheepRepository
{

    public Task<List<CheepViewModel>> ReadAll();
    public Task<List<CheepViewModel>> Read(int pageNumber, int pageSize);

    public Task<List<CheepViewModel>> Query(Expression<Func<Cheep, bool>> condition, int pageNumber, int pageSIze);

    public Task<CheepViewModel> Create(Cheep cheep);
    //Task<UpdateCheepResult> UpdateAsync(UpdateCheepCommand cmd, CancellationToken ct = default);
    public Task<CheepViewModel> Delete(Cheep cheep);
}