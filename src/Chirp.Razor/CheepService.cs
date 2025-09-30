using SimpleDB;
using Chirp.Types;  

public record CheepViewModel(string Author, string Message, string Timestamp);

public interface ICheepService
{
    public List<CheepViewModel> GetCheeps();
    public List<CheepViewModel> GetCheepsFromAuthor(string author);
}

public static class CheepServiceUtils
{
    public static string UnixTimeStampToDateTimeString(double unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp);
        return dateTime.ToString("dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
    }
    
}

public class CheepService : ICheepService
{
    private readonly IDatabaseRepository<Cheep> _database;

    public CheepService(string dbPath)
    {
        _database = DatabaseSessionRegistry<Cheep>.OpenFile(DatabaseType.SQL, dbPath);
    }

    public List<CheepViewModel> GetCheeps()
    {
        return _database.ReadAll().Select(cheep => new CheepViewModel(cheep.Author, cheep.Message, CheepServiceUtils.UnixTimeStampToDateTimeString(cheep.Timestamp))).ToList();
    }

    public List<CheepViewModel> GetCheepsFromAuthor(string author)
    {
        return _database.Query(x => x.Author == author).Select(cheep => new CheepViewModel(cheep.Author, cheep.Message, CheepServiceUtils.UnixTimeStampToDateTimeString(cheep.Timestamp))).ToList();
    }
}