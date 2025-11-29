using Chirp.Core.Application.Contracts;

namespace Chirp.Core.Interfaces;

using Chirp.Core.Entities;

public interface IReplyService
{
    public Task<AppResult<ReplyDTO>> PostReplyAsync(CreateReplyRequest reply);
    public Task<IEnumerable<ReplyDTO>> GetReplies(int CheepId);
}