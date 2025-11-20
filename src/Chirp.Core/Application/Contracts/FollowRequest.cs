namespace Chirp.Core.Application.Contracts;

public record class FollowRequest(int FollowerID, int FolloweeID);