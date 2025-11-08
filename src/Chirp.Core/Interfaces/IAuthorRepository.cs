namespace Chirp.Core.Interfaces;

using Chirp.Core.Entities;
using Chirp.Core.Utils;

public interface IAuthorRepository : IRepository<AuthorDTO>
{
    public Task<Optional<AuthorDTO>> FindAuthorByName(string name);
    public Task<Optional<AuthorDTO>> FindAuthorByEmail(string email);
    public Task AddAuthor(AuthorDTO author);
    public Task Write();
    Task<Optional<AuthorDTO>> FindByIdAsync(int id);
    Task<Optional<AuthorDTO>> FindByNameAsync(string name);
    Task<Optional<AuthorDTO>> FindByEmailAsync(string email);

    Task AddAsync(AuthorDTO author);
}