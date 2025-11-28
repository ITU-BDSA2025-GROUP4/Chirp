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
}
