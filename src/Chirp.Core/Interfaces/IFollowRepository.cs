using Chirp.Core.Application.Contracts;
using Chirp.Core.Entities;

namespace Chirp.Core.Interfaces;

public enum FollowResult
{
    FollowerNotFound,
    FolloweeNotFound,
    AlreadyFollowing,
    UnexpectedError,
    NotFollowing,
    Success,
}

public interface IFollowRepository : IRepository<FollowDTO>
{
    public Task<FollowResult> FollowAsync(FollowRequest followRequest);
    public Task<FollowResult> UnfollowAsync(FollowRequest followRequest);
    public Task<HashSet<string>> GetFollowedAuthorNames(int authorId);
}