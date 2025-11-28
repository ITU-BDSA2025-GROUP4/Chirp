using Chirp.Core.Application.Contracts;
using Chirp.Core.Entities;
using Chirp.Core.Interfaces;
using Chirp.Infrastructure.Data;
using Chirp.Infrastructure.Migrations;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure.Repositories;

public class ReCheepRepository(ChirpDbContext context) : IReCheepRepository
{
    private readonly ChirpDbContext _context = context;

    public async Task<List<ReCheepDTO>> ReadAll()
    {
        return await _context
            .ReCheeps.OrderBy(c => c.Cheep.Timestamp)
            .Select(c => new ReCheepDTO(c.Id, c.CheepId, c.Author.Name))
            .ToListAsync();
    }

    public async Task<IEnumerable<ReCheepDTO>> ReadAsync(int authorId)
    {
        return await _context
            .ReCheeps.Where(c => c.AuthorId == authorId)
            .OrderBy(c => c.Cheep.Timestamp)
            .Select(c => new ReCheepDTO(c.Id, c.CheepId, c.Author.Name))
            .ToListAsync();
    }

    public async Task<IEnumerable<ReCheepDTO>> ReadAsync(string author)
    {
        return await _context
            .ReCheeps.Where(c => c.Author.Name == author)
            .OrderBy(c => c.Cheep.Timestamp)
            .Select(c => new ReCheepDTO(c.Id, c.CheepId, c.Author.Name))
            .ToListAsync();
    }

    public async Task<AppResult<ReCheepDTO>> CreateAsync(CreateReCheepRequst dto)
    {
        if (dto.AuthorId <= 0)
            return AppResult<ReCheepDTO>.Invalid("Invalid author id");

        var reCheep = new ReCheep { AuthorId = dto.AuthorId, CheepId = dto.CheepId };

        await _context.ReCheeps.AddAsync(reCheep);

        await _context.SaveChangesAsync();

        var dtoOut = await ProjectReCheepDtoAsync(reCheep.Id);

        return AppResult<ReCheepDTO>.Created(dtoOut, null);
    }

    private Task<ReCheepDTO> ProjectReCheepDtoAsync(int id) =>
        _context
            .ReCheeps.AsNoTracking()
            .Where(c => c.Id == id)
            .Select(c => new ReCheepDTO(c.Id, c.CheepId, c.Author.Name))
            .SingleAsync();
}
