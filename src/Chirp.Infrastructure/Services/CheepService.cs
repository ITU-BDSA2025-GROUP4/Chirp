using Chirp.Core.Application.Contracts;

namespace Chirp.Infrastructure.Services;

using Chirp.Core.Interfaces;
using Chirp.Core.Entities;
using System.Collections.Generic;

public class CheepService : ICheepService
{
    private readonly ICheepRepository _repository;

    public CheepService(ICheepRepository repository)
    {
        _repository = repository;
    }

    public async Task<AppResult<CheepDTO>> PostCheepAsync(CreateCheepRequest cheep)
        => await _repository.CreateAsync(cheep);

    public async Task<IEnumerable<CheepDTO>> GetCheeps(int page, int pageSize)
    {
        return await _repository.ReadAsync(page, pageSize);
    }

    public async Task<IEnumerable<CheepDTO>> GetCheepsFromAuthor(string author, int page, int pageSize)
    {
        return await _repository.QueryAsync(c => c.Author.Name == author, page, pageSize);
    }

    public async Task<List<CheepDTO>> GetCheepsWrittenByAuthorAndFollowedAuthors(int authorId, int pageNumber, int pageSize)
    {
        return await _repository.GetCheepsWrittenByAuthorAndFollowedAuthors(authorId, pageNumber, pageSize);
    }
}