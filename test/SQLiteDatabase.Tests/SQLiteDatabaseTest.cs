namespace SQLiteDatabase.Tests;

using Chirp.Types;
using SimpleDB;
using Utils;

public class SQLiteDatabaseTest
{

    private static Cheep[] DataSetA() {

        var cheepData = new Cheep[]{
            new Cheep("bob",    "hello",                              100 ),
            new Cheep("tom",    "what's up?",                         500 ),
            new Cheep("bob",    "not much, just writing a unit test", 1000),
            new Cheep("tom",    "what's a unit test?",                1500),
            new Cheep("carl",   "no way he just asked that",          2500),
            new Cheep("bob",    "pain",                               5000),
            new Cheep("carl",   "what?",                              5001),
            new Cheep("tom",    "what?",                              5002),
            new Cheep("admin",  "what?",                              5003),

        };

        return cheepData;
    }

    [Fact]
    public void NoResultsGivenEmptyDatabase()
    {
        IDatabaseRepository<Cheep> db = DatabaseSessionRegistry<Cheep>.OpenInMemory("" + Id.Generate());

        int expected = 0;

        Assert.Equal(db.ReadAll().Count(), expected);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void OneEntryGivesOneResult(int datasetIndex)
    {
        var data = DataSetA();
        IDatabaseRepository<Cheep> db = DatabaseSessionRegistry<Cheep>.OpenInMemory("" + Id.Generate());

        int expectedCount = 1;
        Cheep expectedCheep = data[datasetIndex];

        db.Store(expectedCheep);

        var query = db.ReadAll();
        Assert.Equal(query.Count(), expectedCount);
        Assert.True(query.First().Equals(expectedCheep));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void OneEntryGivesOneResultWithWrite(int datasetIndex)
    {
        var data = DataSetA();
        IDatabaseRepository<Cheep> db = DatabaseSessionRegistry<Cheep>.OpenInMemory("" + Id.Generate());

        int expectedCount = 1;
        Cheep expectedCheep = data[datasetIndex];

        db.Store(expectedCheep);

        db.Write(); // Difference between previous test is that we write to a file

        var query = db.ReadAll();
        Assert.Equal(query.Count(), expectedCount);
        Assert.True(query.First().Equals(expectedCheep));
    }

    [Fact]
    public void MultipleEntries()
    {
        IDatabaseRepository<Cheep> db = DatabaseSessionRegistry<Cheep>.OpenInMemory("" + Id.Generate());

        var data = DataSetA();
        int expectedCount = data.Count();

        foreach(var cheep in data)
            db.Store(cheep);

        var query = db.ReadAll();
        Assert.Equal(query.Distinct().Count(), expectedCount);

        foreach(var cheep in query)
            Assert.Contains(cheep, data);
    }

    [Fact]
    public void MultipleEntriesWrite()
    {
        IDatabaseRepository<Cheep> db = DatabaseSessionRegistry<Cheep>.OpenInMemory("" + Id.Generate());

        var data = DataSetA();
        int expectedCount = data.Count();

        foreach(var cheep in data)
            db.Store(cheep);

        db.Write(); // Difference between previous is that we write to a file

        var query = db.ReadAll();
        Assert.Equal(query.Distinct().Count(), expectedCount);

        foreach(var cheep in query)
            Assert.Contains(cheep, data);
    }

    [Fact]
    public void ReadWithLimit()
    {
        IDatabaseRepository<Cheep> db = DatabaseSessionRegistry<Cheep>.OpenInMemory("" + Id.Generate());

        var data = DataSetA();
        int expectedCount = data.Count() / 2;

        foreach(var cheep in data)
            db.Store(cheep);

        var query = db.Read(expectedCount);
        Assert.Equal(query.Distinct().Count(), expectedCount);

        foreach(var cheep in query)
            Assert.Contains(cheep, data);
    }

    [Theory]
    [InlineData("bob")]
    [InlineData("tom")]
    [InlineData("invalid")]
    public void QueryEntiresByAuthor(string author)
    {
        IDatabaseRepository<Cheep> db = DatabaseSessionRegistry<Cheep>.OpenInMemory("" + Id.Generate());

        var data = DataSetA();
        var expectedData = data.Where(x => x.Author == author);
        int expectedCount = expectedData.Count();

        foreach(var cheep in data)
            db.Store(cheep);

        var query = db.Query(x => x.Author == author);

        Assert.Equal(query.Distinct().Count(), expectedCount);
        foreach(var cheep in query)
            Assert.Contains(cheep, expectedData);

    }


}