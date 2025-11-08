namespace Chirp.Core.Interfaces;

using Chirp.Core.Utils;
using Chirp.Core.Entities;

public interface IAuthorService
{
    Task<IReadOnlyList<AuthorDTO>> ReadAllAsync();

    Task<Optional<AuthorDTO>> FindByIdAsync(int id);
    Task<Optional<AuthorDTO>> FindByNameAsync(string name);
    Task<Optional<AuthorDTO>> FindByEmailAsync(string email);
}