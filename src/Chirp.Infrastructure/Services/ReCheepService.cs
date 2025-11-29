using Chirp.Core.Application.Contracts;
using Chirp.Core.Entities;
using Chirp.Core.Interfaces;

namespace Chirp.Infrastructure.Services;

public class ReCheepService : IReCheepService
{
    private readonly IReCheepRepository _repository;

    public ReCheepService(IReCheepRepository repository)
    {
        _repository = repository;
    }

    public async Task<AppResult<ReCheepDTO>> PostReCheepAsync(CreateReCheepRequst dto) =>
        await _repository.CreateAsync(dto);

    public async Task<IEnumerable<ReCheepDTO>> GetReCheeps(int authorId) =>
        await _repository.ReadAsync(authorId);

    public async Task<IEnumerable<ReCheepDTO>> GetReCheeps(string author) =>
        await _repository.ReadAsync(author);

    public async Task<IEnumerable<ReCheepDTO>> GetReCheeps(int page, int pageSize) =>
        await _repository.ReadAsync(page, pageSize);

    public async Task<IEnumerable<ReCheepDTO>> ReadAll() => await _repository.ReadAll();
}
