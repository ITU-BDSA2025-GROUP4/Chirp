using System.Linq.Expressions;

using Chirp.Core.Application;
using Chirp.Core.Application.Contracts;
using Chirp.Core.Entities;

namespace Chirp.Core.Interfaces;

public interface ICheepRepository : IRepository<CheepDTO>
{
    public Task<List<CheepDTO>> ReadAsync(int pageNumber, int pageSize);
    public Task<List<CheepDTO>> QueryAsync(Expression<Func<Cheep, bool>> condition, int pageNumber, int
        pageSize);
    public Task<AppResult<CheepDTO>> CreateAsync(CreateCheepRequest dto);
    public Task<AppResult<CheepDTO>> UpdateAsync(UpdateCheepRequest dto);
    public Task<AppResult> DeleteAsync(DeleteCheepRequest dto);
}