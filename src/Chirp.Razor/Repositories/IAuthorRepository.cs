using Chirp.Razor.Models;

using System.Linq.Expressions;

using Utils;

namespace Chirp.Razor.Repositories;

public interface IAuthorRepostiory
{
    public Task<Optional<AuthorDTO>> FindAuthorByName(string name);
    public Task<Optional<AuthorDTO>> FindAuthorByEmail(string email);
    public void AddAuthor(AuthorDTO author);
}