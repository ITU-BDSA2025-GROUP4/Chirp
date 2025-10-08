using Chirp.Razor.Models;
using Chirp.Razor.Data;

using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

using Utils;

namespace Chirp.Razor.Repositories;

public class AuthorRepository : IAuthorRepostiory
{
    private readonly ChirpDbContext _context;

    public AuthorRepository(ChirpDbContext context)
    {
        _context = context;
    }

    public async Task<Optional<AuthorDTO>> FindAuthorByName(string name)
    {
        return await _context.Authors
            .Where(a => a.Name == name)
            .Select(x => Optional.Of<AuthorDTO>(new AuthorDTO(x.Name, x.Email)))
            .DefaultIfEmpty(Optional.Empty<AuthorDTO>())
            .FirstOrDefaultAsync();
    }

    public async Task<Optional<AuthorDTO>> FindAuthorByEmail(string email)
    {
        return await _context.Authors
            .Where(a => a.Name == email)
            .Select(x => Optional.Of<AuthorDTO>(new AuthorDTO(x.Name, x.Email)))
            .DefaultIfEmpty(Optional.Empty<AuthorDTO>())
            .FirstOrDefaultAsync();
    }

    public async void AddAuthor(AuthorDTO author)
    {
        var newAuthor = new Author();
        newAuthor.Name = author.Name;
        newAuthor.Email = author.Email;

        await _context.Authors.AddAsync(newAuthor);
    }
}