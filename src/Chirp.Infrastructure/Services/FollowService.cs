

using Chirp.Core.Application.Contracts;
using Chirp.Core.Interfaces;

namespace Chirp.Infrastructure.Services;

public class FollowService(IFollowRepository repository) : IFollowService
{
    private readonly IFollowRepository _repository = repository;

    // Please note that we kind of just ignore most "errors" and say that they succeed, since the operations are idempotent
    // i.e. if you follow someone you are already following, we just say it suceeds, since the follow was not added again
    // same goes for unfollow etc
    //
    // Also it is not possible for an Author to follow/unfollow themselves

    public async Task<bool> FollowAuthorAsync(FollowRequest followRequest)
    {
        if (followRequest.FollowerID == followRequest.FolloweeID)
        {
            return false;
        }

        var result = await _repository.FollowAsync(followRequest);

        return result switch
        {
            FollowResult.FollowerNotFound or FollowResult.FolloweeNotFound or FollowResult.UnexpectedError => false,
            _ => true,
        };
    }

    public async Task<HashSet<string>> GetFollowedAuthorNames(int authorId)
    {
        return await _repository.GetFollowedAuthorNames(authorId);
    }

    public async Task<bool> UnfollowAuthorAsync(FollowRequest followRequest)
    {
        if (followRequest.FollowerID == followRequest.FolloweeID)
        {
            return false;
        }

        var result = await _repository.UnfollowAsync(followRequest);

        return result switch
        {
            FollowResult.FollowerNotFound or FollowResult.FolloweeNotFound or FollowResult.UnexpectedError => false,
            _ => true,
        };
    }
}