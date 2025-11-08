namespace Chirp.Core.Interfaces;

using Chirp.Core.Entities;
using Chirp.Core.Utils;

public interface IAuthorRepository : IRepository<AuthorDTO>
{
    Task<Optional<AuthorDTO>> FindByIdAsync(int id);
    Task<Optional<AuthorDTO>> FindByNameAsync(string name);
    Task<Optional<AuthorDTO>> FindByEmailAsync(string email);

    Task AddAsync(AuthorDTO author);
}