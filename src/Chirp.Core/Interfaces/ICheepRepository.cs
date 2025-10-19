using System.Linq.Expressions;

using Chirp.Core.Application;
using Chirp.Core.Application.Contracts;
using Chirp.Core.Entities;

namespace Chirp.Core.Interfaces;

public interface ICheepRepository : IRepository<CheepDTO>
{
    public Task<List<CheepDTO>> Read(int pageNumber, int pageSize);
    public Task<List<CheepDTO>> Query(Expression<Func<Cheep, bool>> condition, int pageNumber, int 
        pageSIze);
    public Task<AppResult<CheepDTO>> Create(CheepCreateDto dto);
    public Task<AppResult<CheepDTO>> Update(CheepUpdateDTO dto);

}   