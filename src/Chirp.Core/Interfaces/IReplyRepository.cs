using System.Linq.Expressions;

using Chirp.Core.Application;
using Chirp.Core.Application.Contracts;
using Chirp.Core.Entities;

namespace Chirp.Core.Interfaces;

public interface IReplyRepository : IRepository<ReplyDTO>
{
    public Task<AppResult<ReplyDTO>> CreateAsync(CreateReplyRequest dto);
    public Task<IEnumerable<ReplyDTO>> ReadAsync(int CheepId);
}