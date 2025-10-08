using Chirp.Razor.Models;
using Chirp.Razor.Repositories;

using Utils;

public interface IAuthorService
{
    public Task<IEnumerable<AuthorDTO>> GetAuthors();
    public Task<Optional<AuthorDTO>> GetAuthorByName(string name);
    public Task<Optional<AuthorDTO>> GetAuthorByEmail(string email);
}

public class AuthorService : IAuthorService
{
    private readonly IAuthorRepository _repository;

    public AuthorService(IAuthorRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<AuthorDTO>> GetAuthors()
    {
        return await _repository.ReadAll();
    }

    public async Task<Optional<AuthorDTO>> GetAuthorByEmail(string email)
    {
        return await _repository.FindAuthorByEmail(email);
    }

    public async Task<Optional<AuthorDTO>> GetAuthorByName(string name)
    {
        return await _repository.FindAuthorByName(name);
    }

}