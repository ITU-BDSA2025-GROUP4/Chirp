namespace Chirp.Core.Interfaces;

using Chirp.Core.Entities;

public interface ICheepService
{
    public Task<bool> AddCheep(CheepDTO cheep);
    public Task<IEnumerable<CheepDTO>> GetCheeps(int page, int pageSize);
    public Task<IEnumerable<CheepDTO>> GetCheepsFromAuthor(string author, int page, int pageSize);
}