using Chirp.Razor.Models;

using System.Linq.Expressions;

using Utils;

namespace Chirp.Razor.Repositories;

public interface ICheepRepository
{
    public Task<Optional<AuthorDTO>> FindAuthorByName(string name);
    public Task<Optional<AuthorDTO>> FindAuthorByEmail(string email);

    public void AddAuthor(AuthorDTO author);

    public Task<List<CheepDTO>> ReadAll();
    public Task<List<CheepDTO>> Read(int pageNumber, int pageSize);
    public Task<List<CheepDTO>> Query(Expression<Func<Cheep, bool>> condition, int pageNumber, int pageSIze);
    //todo: add createcheep, updatecheep, remove cheetp osv.
}