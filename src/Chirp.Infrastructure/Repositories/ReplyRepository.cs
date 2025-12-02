using Microsoft.EntityFrameworkCore;

using System.Linq.Expressions;

using Chirp.Core.Application;
using Chirp.Core.Application.Contracts;
using Chirp.Core.Entities;
using Chirp.Core.Interfaces;
using Chirp.Infrastructure.Data;
using Chirp.Core.Utils;
using Chirp.Infrastructure.Utils;

namespace Chirp.Infrastructure.Repositories;

public class ReplyRepository(ChirpDbContext context) : IReplyRepository
{
    private readonly ChirpDbContext _context = context;

    public async Task<List<ReplyDTO>> ReadAll()
    {
        return await _context.Replies
            .OrderBy(c => c.Timestamp)
            .Select(c => new ReplyDTO(c.Id, c.CheepId, c.Author.Name, c.Text,
                TimestampUtils.DateTimeTimeStampToDateTimeString(c.Timestamp)))
            .ToListAsync();
    }

    public async Task<IEnumerable<ReplyDTO>> ReadAsync(int CheepId)
    {
        return await _context.Replies
            .Where(c => c.CheepId == CheepId)
            .OrderBy(c => c.Timestamp)
            .Select(c => new ReplyDTO(c.Id, c.CheepId, c.Author.Name, c.Text,
                TimestampUtils.DateTimeTimeStampToDateTimeString(c.Timestamp)))
            .ToListAsync();
    }

    public async Task<AppResult<ReplyDTO>> CreateAsync(CreateReplyRequest dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Text))
            return AppResult<ReplyDTO>.Invalid("text must be provided");

        if (dto.AuthorId <= 0)
            return AppResult<ReplyDTO>.Invalid("invalid author id");

        var reply = new Reply
        {
            Text = dto.Text.Trim(),
            AuthorId = dto.AuthorId,
            Timestamp = DateTime.UtcNow,
            CheepId = dto.CheepId
        };

        await _context.Replies.AddAsync(reply);
//        _context.Entry(reply).Property("ETag").CurrentValue = ETagUtils.NewValue();

        await _context.SaveChangesAsync();

        var dtoOut = await ProjectReplyDtoAsync(reply.Id);

 //       var etagBytes = (byte[])_context.Entry(reply).Property("ETag").CurrentValue!;
  //      var etag = ETagUtils.ToBase64(etagBytes);

        return AppResult<ReplyDTO>.Created(dtoOut, null);
    }

    private Task<ReplyDTO> ProjectReplyDtoAsync(int id) =>
        _context.Replies
            .AsNoTracking()
            .Where(c => c.Id == id)
            .Select(c => new ReplyDTO(
                c.Id,
                c.CheepId,
                c.Author.Name,
                c.Text,
                TimestampUtils.DateTimeTimeStampToDateTimeString(c.Timestamp)))
            .SingleAsync();
}