namespace Chirp.Core.Entities;

public record ReplyDTO(int Id, int CheepId, string Author, string Text, string Timestamp);