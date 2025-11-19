using Chirp.Core.Application.Contracts;
using Chirp.Core.Entities;
using Chirp.Core.Interfaces;
using Chirp.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure.Repositories;

public class FollowRepository(ChirpDbContext context) : IFollowRepository
{

    private readonly ChirpDbContext _context = context;

    public async Task<List<FollowDTO>> ReadAll()
    {
        return await _context.Follows
            .Select(f => new FollowDTO(f.FollowerFK, f.FolloweeFK))
            .ToListAsync();
    }

    public async Task<FollowResult> FollowAsync(FollowRequest followRequest)
    {
        var authorFollower = await _context.Authors
            .FirstOrDefaultAsync(a => a.Id == followRequest.FollowerID);

        if (authorFollower == null) return FollowResult.FollowerNotFound;


        var authorFollowee = await _context.Authors
            .FirstOrDefaultAsync(a => a.Id == followRequest.FolloweeID);

        if (authorFollowee == null) return FollowResult.FolloweeNotFound;

        //TODO: Figure out if we can do this more efficiently :) In RAW sql we could do something like ON CONFLICT DO NOTHING, but i couldnt find this .. :(
        var existingFollow = await _context.Follows
            .AnyAsync(f => f.FollowerFK == followRequest.FollowerID && f.FolloweeFK == followRequest.FolloweeID);

        if (existingFollow) return FollowResult.AlreadyFollowing;

        var follow = new Follow
        {
            Follower = authorFollower,
            Followee = authorFollowee,
        };

        await _context.Follows.AddAsync(follow);
        await _context.SaveChangesAsync();


        return FollowResult.Success;
    }

    public async Task<FollowResult> UnfollowAsync(FollowRequest followRequest)
    {
        var authorFollower = await _context.Authors
            .FirstOrDefaultAsync(a => a.Id == followRequest.FollowerID);

        if (authorFollower == null) return FollowResult.FollowerNotFound;


        var authorFollowee = await _context.Authors
            .FirstOrDefaultAsync(a => a.Id == followRequest.FolloweeID);

        if (authorFollowee == null) return FollowResult.FolloweeNotFound;

        var follow = await _context.Follows
            .FirstOrDefaultAsync(f => f.FollowerFK == followRequest.FollowerID && f.FolloweeFK == followRequest.FolloweeID);

        if (follow == null)
            return FollowResult.NotFollowing;

        _context.Follows.Remove(follow);
        await _context.SaveChangesAsync();

        return FollowResult.Success;
    }

    public async Task<HashSet<string>> GetFollowedAuthorNames(int authorId)
    {
        return await _context.Follows
            .Where(f => f.FollowerFK == authorId)
            .Select(f => f.Followee.Name)
            .ToHashSetAsync();
    }
}