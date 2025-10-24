namespace Chirp.Infrastructure.Services;

using Chirp.Core.Interfaces;
using Chirp.Core.Entities;

public class CheepService : ICheepService
{
    private readonly ICheepRepository _repository;

    public CheepService(ICheepRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> AddCheep(CheepDTO cheep) {
        return await _repository.Add(cheep);
    }

    public async Task<IEnumerable<CheepDTO>> GetCheeps(int page, int pageSize)
    {
        return await _repository.Read(page, pageSize);
    }

    public async Task<IEnumerable<CheepDTO>> GetCheepsFromAuthor(string author, int page, int pageSize)
    {
        return await _repository.Query(c => c.Author.Name == author, page, pageSize);
    }
}