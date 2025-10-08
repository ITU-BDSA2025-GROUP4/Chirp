using Chirp.Core.Entities;

namespace Chirp.Core.Interfaces;

public interface ICheepService
{
    public Task<IEnumerable<CheepDTO>> GetCheeps(int page, int pageSize);
    public Task<IEnumerable<CheepDTO>> GetCheepsFromAuthor(string author, int page, int pageSize);
}