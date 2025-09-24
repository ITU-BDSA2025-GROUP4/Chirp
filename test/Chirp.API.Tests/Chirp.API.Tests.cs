namespace Chirp.API.Tests;

using Xunit;

using SimpleDB;
using APICore;
using Chirp.Types;

public class APICoreUnitTest
{

    private Dictionary<string, string> EmptyQuery() {
        return new Dictionary<string, string>();
    }

    [Fact]
    public void NoResultGivenEmptyDatabase()
    {
        CsvDatabase<Cheep> db = new();
        APICore core = new APICore(db);

        var query = EmptyQuery();

        var numberOfCheeps = core.Cheeps(query).Count();

        Assert.Equal(numberOfCheeps, 0);
    }

    [Theory]
    [InlineData("bob", "hello", 1)]
    [InlineData("tom", "hello world", 2)]
    [InlineData("joe", "lorem ipsum", 100)]
    public void GetsCheeped(string name, string message, int timestamp)
    {
        CsvDatabase<Cheep> db = new();
        APICore core = new APICore(db);

        var query = EmptyQuery();
        query.Add("author", name);
        query.Add("message", message);
        query.Add("timestamp", timestamp.ToString());

        var result = core.Cheep(query);

        Assert.Equal(result, "Cheep'ed");

        var queryResult = core.Cheeps(EmptyQuery());

        Assert.Equal(queryResult.Count(), 1);

        var cheep = queryResult.First();
        
        Assert.Equal(cheep.Author, name);
        Assert.Equal(cheep.Message, message);
        Assert.Equal(cheep.Timestamp, timestamp);
    }

    [Fact]
    public void GetsAfterQuery()
    {
        CsvDatabase<Cheep> db = new();
        APICore core = new APICore(db);

        var cheepData = new Cheep[]{
            new Cheep("bob", "hello", 1),
            new Cheep("tom", "what's up?", 2),
            new Cheep("bob", "not much, just writing a unit test", 3),
            new Cheep("tom", "what's a unit test?", 4),
            new Cheep("carl", "no way he just asked that", 5),
            new Cheep("tom", "pain", 6),
            new Cheep("carl", "what?", 7),
            new Cheep("bob", "what?", 8),
            new Cheep("admin", "what?", 9),

        };

        var query = EmptyQuery();
        foreach(Cheep i in cheepData) {
            query.Clear();

            query.Add("author", i.Author);
            query.Add("message", i.Message);
            query.Add("timestamp", i.Timestamp.ToString());

            Assert.Equal(core.Cheep(query), "Cheep'ed");
        }

        

        var result = core.Cheep(query);

        Assert.Equal(result, "Cheep'ed");

        var queryResult = core.Cheeps(EmptyQuery());

        Assert.Equal(queryResult.Count(), 1);

        var cheep = queryResult.First();
        
        Assert.Equal(cheep.Author, name);
        Assert.Equal(cheep.Message, message);
        Assert.Equal(cheep.Timestamp, timestamp);
    }

}