namespace Chirp.Core.Interfaces;

using Chirp.Core.Entities;
using Chirp.Core.Utils;

public interface IAuthorRepository : IRepository<AuthorDTO>
{
    public Task<Optional<AuthorDTO>> FindByNameAsync(string name);
    public Task<Optional<AuthorDTO>> FindByEmailAsync(string email);
    public Task<Optional<AuthorDTO>> FindByIdAsync(int id);
    public Task AddAuthorAsync(AuthorDTO author);
    public Task<bool> DeleteAuthor(AuthorDTO author);

    public Task Write();
}