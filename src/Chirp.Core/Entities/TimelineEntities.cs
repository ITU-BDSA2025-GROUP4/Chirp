using Chirp.Core.Utils;

namespace Chirp.Core.Entities;

public enum TimelineType
{
    Cheep,
    ReCheep,
}

public class TimelineEntities
{
    public TimelineType Type { get; set; }

    public CheepDTO Cheep { get; set; } = null!;
    public Optional<ReCheepDTO> ReCheep { get; set; }
}
