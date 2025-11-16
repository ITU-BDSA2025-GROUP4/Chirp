using Chirp.Core.Application.Contracts;

namespace Chirp.Core.Interfaces;

public interface IFollowService
{
    public Task<bool> FollowAuthorAsync(FollowRequest followRequest);
    public Task<bool> UnfollowAuthorAsync(FollowRequest followRequest);
}