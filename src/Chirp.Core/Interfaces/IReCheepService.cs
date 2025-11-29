using Chirp.Core.Application.Contracts;
using Chirp.Core.Entities;

namespace Chirp.Core.Interfaces;

public interface IReCheepService
{
    public Task<AppResult<ReCheepDTO>> PostReCheepAsync(CreateReCheepRequst reCheep);
    public Task<IEnumerable<ReCheepDTO>> GetReCheeps(int authorId);
    public Task<IEnumerable<ReCheepDTO>> GetReCheeps(string author);
    public Task<IEnumerable<ReCheepDTO>> GetReCheeps(int page, int pageSize);
    public Task<IEnumerable<ReCheepDTO>> ReadAll();
    // public Task<IEnumerable<ReCheepDTO>> GetReCheepFromCheep(int cheepId);
    // public Task<IEnumerable<ReCheepDTO>> GetReCheepsFromAuthor(int authorId);
}