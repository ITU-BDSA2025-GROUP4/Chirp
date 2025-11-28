using Chirp.Core.Application.Contracts;
using Chirp.Core.Entities;

namespace Chirp.Core.Interfaces;

public interface IReCheepRepository : IRepository<ReCheepDTO>
{
    public Task<AppResult<ReCheepDTO>> CreateAsync(CreateReCheepRequst dto);
    public Task<IEnumerable<ReCheepDTO>> ReadAsync(int authorId);
}
