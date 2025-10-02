using Chirp.Razor.Models;
using Chirp.Razor.Repositories;


public interface ICheepService
{
    public Task<IEnumerable<CheepViewModel>> GetCheeps(int page, int pageSize);
    public Task<IEnumerable<CheepViewModel>> GetCheepsFromAuthor(string author, int page, int pageSize);
}

public class CheepService : ICheepService
{
    private readonly ICheepRepository _repository;

    public CheepService(ICheepRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<CheepViewModel>> GetCheeps(int page, int pageSize)
    {
        return await _repository.Read(page, pageSize);
    }

    public async Task<IEnumerable<CheepViewModel>> GetCheepsFromAuthor(string author, int page, int pageSize)
    {
        return await _repository.Query(c => c.Author.Name == author, page, pageSize);
    }
}