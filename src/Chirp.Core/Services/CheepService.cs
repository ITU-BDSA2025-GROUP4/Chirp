using Chirp.Core.Interfaces;
using Chirp.Core.Entities;

namespace Chirp.Core.Services;

public class CheepService : ICheepService
{
    private readonly ICheepRepository _repository;

    public CheepService(ICheepRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<CheepDTO>> GetCheeps(int page, int pageSize)
    {
        return await _repository.ReadAsync(page, pageSize);
    }

    public async Task<IEnumerable<CheepDTO>> GetCheepsFromAuthor(string author, int page, int pageSize)
    {
        return await _repository.QueryAsync(c => c.Author.Name == author, page, pageSize);
    }
}