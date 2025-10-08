using Chirp.Razor.Models;

using System.Linq.Expressions;

using Utils;

namespace Chirp.Razor.Repositories;

public interface IAuthorRepository : IRepository<AuthorDTO>
{
    public Task<Optional<AuthorDTO>> FindAuthorByName(string name);
    public Task<Optional<AuthorDTO>> FindAuthorByEmail(string email);
    public Task AddAuthor(AuthorDTO author);
}