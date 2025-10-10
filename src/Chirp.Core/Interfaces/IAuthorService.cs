namespace Chirp.Core.Interfaces;

using Chirp.Core.Utils;
using Chirp.Core.Entities;

public interface IAuthorService
{
    public Task<IEnumerable<AuthorDTO>> GetAuthors();
    public Task<Optional<AuthorDTO>> GetAuthorByName(string name);
    public Task<Optional<AuthorDTO>> GetAuthorByEmail(string email);
}