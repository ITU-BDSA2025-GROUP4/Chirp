using Chirp.Core.Application.Contracts;

namespace Chirp.Infrastructure.Services;

using Chirp.Core.Interfaces;
using Chirp.Core.Entities;
using System.Collections.Generic;

public class ReplyService : IReplyService
{
    private readonly IReplyRepository _repository;

    public ReplyService(IReplyRepository repository)
    {
        _repository = repository;
    }

    public async Task<AppResult<ReplyDTO>> PostReplyAsync(CreateReplyRequest dto)
        => await _repository.CreateAsync(dto);

    public async Task<IEnumerable<ReplyDTO>> GetReplies(int CheepId)
        => await _repository.ReadAsync(CheepId);
}