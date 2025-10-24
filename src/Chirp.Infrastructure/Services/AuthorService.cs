namespace Chirp.Core.Services;

using Chirp.Core.Interfaces;
using Chirp.Core.Entities;
using Chirp.Core.Utils;

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