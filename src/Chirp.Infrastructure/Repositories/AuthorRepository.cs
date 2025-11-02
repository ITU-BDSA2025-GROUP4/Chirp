namespace Chirp.Infrastructure.Repositories;

using Chirp.Core.Entities;
using Chirp.Core.Interfaces;
using Chirp.Core.Utils;

using Chirp.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;

public class AuthorRepository : IAuthorRepository
{
    private readonly ChirpDbContext _context;

    public AuthorRepository(ChirpDbContext context)
    {
        _context = context;
    }

    public async Task<List<AuthorDTO>> ReadAll()
    {
        return await _context.Authors
            .Select(x => new AuthorDTO(x.Name, x.Email))
            .ToListAsync();
    }

    public async Task<Optional<AuthorDTO>> FindAuthorByName(string name)
    {
        var tmp = await _context.Authors
            .Where(a => a.Name == name)
            .Select(x => new AuthorDTO(x.Name, x.Email))
            .ToListAsync();

        if (tmp.Count() == 0) return Optional.Empty<AuthorDTO>();
        else return Optional.Of<AuthorDTO>(tmp.First());
    }

    public async Task<Optional<AuthorDTO>> FindAuthorByEmail(string email)
    {

        var tmp = await _context.Authors
            .Where(a => a.Email == email)
            .Select(x => new AuthorDTO(x.Name, x.Email))
            .ToListAsync();

        if (tmp.Count() == 0) return Optional.Empty<AuthorDTO>();
        else return Optional.Of<AuthorDTO>(tmp.First());
    }

    public async Task AddAuthor(AuthorDTO author)
    {
        var newAuthor = new Author
        {
            Name = author.Name,
            Email = author.Email,
            Cheeps = new List<Cheep>()
        };

        newAuthor.Name = author.Name;
        newAuthor.Email = author.Email;

        await _context.Authors.AddAsync(newAuthor);
        await _context.SaveChangesAsync();
    }
}