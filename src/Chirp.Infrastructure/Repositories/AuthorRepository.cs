using Chirp.Core.Entities;
using Chirp.Core.Interfaces;
using Chirp.Core.Utils;
using Chirp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Chirp.Infrastructure.Repositories;

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
            .AsNoTracking()
            .Select(x => new AuthorDTO(x.Name, x.Email, x.Id))
            .ToListAsync();
    }

    public async Task<Optional<AuthorDTO>> FindByIdAsync(int id)
        => await FindAsync(a => a.Id == id);

    public async Task<Optional<AuthorDTO>> FindByNameAsync(string name)
        => await FindAsync(a => a.Name == name);

    public async Task<Optional<AuthorDTO>> FindByEmailAsync(string email)
        => await FindAsync(a => a.Email == email);

    private async Task<Optional<AuthorDTO>> FindAsync(Expression<Func<Author, bool>> predicate)
    {
        var entity = await _context.Authors
            .AsNoTracking()
            .Where(predicate)
            .Select(x => new AuthorDTO(x.Name, x.Email, x.Id))
            .FirstOrDefaultAsync();

        return entity is null ? Optional.Empty<AuthorDTO>() : Optional.Of(entity);
    }

    public async Task AddAuthorAsync(AuthorDTO author)
    {
        var newAuthor = new Author
        {
            Id = author.Id,
            Name = author.Name,
            Email = author.Email,
            Cheeps = new List<Cheep>()
        };

        await _context.Authors.AddAsync(newAuthor);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> DeleteAuthor(AuthorDTO author)
    {
        List<Author> result =
            await _context.Authors.Where(a => a.Email == author.Email).ToListAsync();

        // TODO - report error
        if(result.Count() != 1) {
            return false;
        }

        List<Cheep> cheeps = 
            await _context.Cheeps.Where(c => c.Author.Email == author.Email).ToListAsync();

        List<Reply> replies = 
            await _context.Replies.Where(r => r.AuthorId == author.Id).ToListAsync();

        _context.RemoveRange(cheeps);
        _context.RemoveRange(replies);
        _context.Remove(result[0]);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task Write() => await _context.SaveChangesAsync();

    public async Task<Optional<Author>> GetConcreteAuthorAsync(string email)
    {
        var author = await _context.Authors
            .Where(a => a.Email == email)
            .FirstOrDefaultAsync();

        if(author == null)
        {
            return Optional.Empty<Author>();
        }
        return Optional.Of<Author>(author);
    }
}