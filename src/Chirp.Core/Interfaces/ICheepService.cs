using Chirp.Core.Application.Contracts;

namespace Chirp.Core.Interfaces;

using Chirp.Core.Entities;

public interface ICheepService
{
    public Task<AppResult<CheepDTO>> PostCheepAsync(CreateCheepRequest cheep);
    public Task<IEnumerable<CheepDTO>> GetCheeps(int page, int pageSize);
    public Task<IEnumerable<CheepDTO>> GetCheepsFromAuthor(string author, int page, int pageSize);
}