namespace Chirp.API.Tests;

using Xunit;

using SimpleDB;
using APICore;
using Utils;
using Chirp.Types;

public class APICoreUnitTest
{
    private static Dictionary<string, string> EmptyQuery()
    {
        return new Dictionary<string, string>();
    }

    private static APICore EmptyAPI()
    {
        IDatabaseRepository<Cheep> db = DatabaseSessionRegistry<Cheep>.OpenInMemory(""+Id.Generate());

        APICore core = new APICore(db);
        return core;
    }

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

    private static (APICore, Cheep[]) APIDataSetA()
    {
        APICore core = EmptyAPI();

        var cheepData = DataSetA();

        var query = EmptyQuery();
        foreach (Cheep i in cheepData)
        {
            query.Clear();

            query.Add("author", i.Author);
            query.Add("message", i.Message);
            query.Add("timestamp", i.Timestamp.ToString());

            var expectedStatus = APICore.CheepStatusCode.SUCCESS;
            Assert.Equal(core.Cheep(query), expectedStatus);
        }

        return (core, cheepData);
    }

    private static bool QueryResultMatchesData(IEnumerable<Cheep> queryResult, IEnumerable<Cheep> expected) 
    {
        return (queryResult.Distinct().Count() ==  expected.Count())
            &&
            queryResult.Select(x => expected.Where(y => x.Equals(y)).Count() == 1).Aggregate(true, (a, b) => a && b);
    }

    [Fact]
    public void NoResultGivenEmptyDatabase()
    {
        APICore core = EmptyAPI();
        var query = EmptyQuery();

        var numberOfCheeps = core.Cheeps(query).Count();

        var expected = 0;
        Assert.Equal(numberOfCheeps, expected);
    }

    [Theory]
    [InlineData("bob", "hello", 1)]
    [InlineData("tom", "hello world", 2)]
    [InlineData("joe", "lorem ipsum", 100)]
    public void GetsCheeped(string name, string message, int timestamp)
    {
        APICore core = EmptyAPI();

        var query = EmptyQuery();
        query.Add("author", name);
        query.Add("message", message);
        query.Add("timestamp", timestamp.ToString());

        var result = core.Cheep(query);

        var expectedStatus = APICore.CheepStatusCode.SUCCESS;
        Assert.Equal(result, expectedStatus);

        var queryResult = core.Cheeps(EmptyQuery());

        var expected = 1;
        Assert.Equal(queryResult.Count(), expected);

        var cheep = queryResult.First();
        Assert.Equal(cheep.Author, name);
        Assert.Equal(cheep.Message, message);
        Assert.Equal(cheep.Timestamp, timestamp);
    }

  [Fact]
  public void QueryAll()
  {
      (APICore core, Cheep[] expectedData) = APIDataSetA();

      var query = EmptyQuery();

      var result = core.Cheeps(query);
      Assert.True(
          QueryResultMatchesData(result, expectedData)
      );

  }

    [Theory]
    [InlineData("bob")]
    [InlineData("tom")]
    [InlineData("carl")]
    [InlineData("admin")]
    public void QueryByUser(string name)
    {
        (APICore core, Cheep[] expectedData) = APIDataSetA();

        var query = EmptyQuery();

        var queryKey = APICore.Query.ToString(APICore.QueryParameter.byUsers);
        query.Add(queryKey, name);

        var result = core.Cheeps(query);
        Assert.True(
            QueryResultMatchesData(result, expectedData.Where(x => x.Author == name))
        );
    }

    [Theory]
    [InlineData("bob", "tom")]
    [InlineData("carl", "admin")]
    [InlineData("admin", "bob", "tom")]
    public void QueryByUsers(params string[] names)
    {
        (APICore core, Cheep[] expectedData) = APIDataSetA();

        var query = EmptyQuery();

        var queryKey = APICore.Query.ToString(APICore.QueryParameter.byUsers);
        query.Add(queryKey, names.Aggregate((x,y) => x + "," + y));

        var result = core.Cheeps(query);
        Assert.True(
            QueryResultMatchesData(result, expectedData.Where(x => names.Contains(x.Author)))
        );
    }



    [Theory]
    [InlineData("bob")]
    [InlineData("tom")]
    [InlineData("carl")]
    [InlineData("admin")]
    public void QueryNotByUser(string name)
    {
        (APICore core, Cheep[] expectedData) = APIDataSetA();

        var query = EmptyQuery();

        var queryKey = APICore.Query.ToString(APICore.QueryParameter.notByUsers);
        query.Add(queryKey, name);

        var result = core.Cheeps(query);
        Assert.True(
            QueryResultMatchesData(result, expectedData.Where(x => x.Author != name))
        );
    }

    [Theory]
    [InlineData("bob", "tom")]
    [InlineData("carl", "admin")]
    [InlineData("admin", "bob", "tom")]
    public void QueryNotByUsers(params string[] names)
    {
        (APICore core, Cheep[] expectedData) = APIDataSetA();

        var query = EmptyQuery();

        var queryKey = APICore.Query.ToString(APICore.QueryParameter.notByUsers);

        query.Add(queryKey, names.Aggregate((x,y) => x + "," + y));
        var expectedOccurences = expectedData.Where(x => !names.Contains(x.Author)).Count();

        var result = core.Cheeps(query);
        Assert.True(
            QueryResultMatchesData(result, expectedData.Where(x => !names.Contains(x.Author)))
        );
    }

    [Theory]
    [InlineData("what")]
    [InlineData("unit test")]
    [InlineData("no")]
    [InlineData("'")]
    public void QueryCheepContains(string needle)
    {
        (APICore core, Cheep[] expectedData) = APIDataSetA();

        var query = EmptyQuery();

        var queryKey = APICore.Query.ToString(APICore.QueryParameter.cheepContains);
        query.Add(queryKey, needle);

        var result = core.Cheeps(query);
        Assert.True(
            QueryResultMatchesData(result, expectedData.Where(x => x.Message.ToLower().Contains(needle)))
        );
    }

    [Theory]
    [InlineData(1)]
    [InlineData(500)]
    [InlineData(1000)]
    [InlineData(1500)]
    [InlineData(2500)]
    [InlineData(5000)]
    [InlineData(5001)]
    [InlineData(5002)]
    [InlineData(5003)]
    [InlineData(10000)]
    public void QueryBeforeTime(long timestamp)
    {
        (APICore core, Cheep[] expectedData) = APIDataSetA();

        var query = EmptyQuery();

        var queryKey = APICore.Query.ToString(APICore.QueryParameter.beforeTime);
        query.Add(queryKey, timestamp.ToString());

        var result = core.Cheeps(query);
        Assert.True(
            QueryResultMatchesData(result, expectedData.Where(x => x.Timestamp < timestamp))
        );
    }

    [Theory]
    [InlineData(1)]
    [InlineData(500)]
    [InlineData(1000)]
    [InlineData(1500)]
    [InlineData(2500)]
    [InlineData(5000)]
    [InlineData(5001)]
    [InlineData(5002)]
    [InlineData(5003)]
    [InlineData(10000)]
    public void QueryAfterTime(long timestamp)
    {
        (APICore core, Cheep[] expectedData) = APIDataSetA();

        var query = EmptyQuery();

        var queryKey = APICore.Query.ToString(APICore.QueryParameter.afterTime);
        query.Add(queryKey, timestamp.ToString());

        var result = core.Cheeps(query);
        Assert.True(
            QueryResultMatchesData(result, expectedData.Where(x => x.Timestamp > timestamp))
        );
    }

    [Theory]
    [InlineData(90, 2000, "bob")]
    [InlineData(5050, 5100, "bob")]
    [InlineData(4000, 5100, "tom")]
    public void QueryAfterMultiple(long afterTime, long beforeTime, string byUser)
    {
        (APICore core, Cheep[] expectedData) = APIDataSetA();

        var query = EmptyQuery();

        var queryKey = APICore.Query.ToString(APICore.QueryParameter.afterTime);
        query.Add(queryKey, afterTime.ToString());

        queryKey = APICore.Query.ToString(APICore.QueryParameter.beforeTime);
        query.Add(queryKey, beforeTime.ToString());

        queryKey = APICore.Query.ToString(APICore.QueryParameter.byUsers);
        query.Add(queryKey, byUser);

        var expectedOccurences = expectedData.Where(
                x => 
                   x.Timestamp > afterTime 
                && x.Timestamp < beforeTime
                && x.Author == byUser
                ).Count();

        var result = core.Cheeps(query);
        Assert.True(
            QueryResultMatchesData(result,

                expectedData.Where(
                x => x.Timestamp > afterTime 
                && x.Timestamp < beforeTime
                && x.Author == byUser
        )));
    }

    [Theory]
    [InlineData("bob", "test", 123)]
    [InlineData("tom", "message with spaces", 54321)]
    public void WriteSingle(string name, string message, long timestamp) 
    {
        APICore core = EmptyAPI();
        var query = EmptyQuery();

        query.Add("author", name);
        query.Add("message", message);
        query.Add("timestamp", timestamp.ToString());

        var result = core.Cheep(query);
        var expected = APICore.CheepStatusCode.SUCCESS;
        Assert.Equal(result, expected);

        var resultCollection = core.Cheeps(EmptyQuery());
        var expectedSize = 1;
        Assert.Equal(resultCollection.Count(), expectedSize);

        var cheep = resultCollection.First();
        Assert.Equal(cheep.Author, name);
        Assert.Equal(cheep.Message, message);
        Assert.Equal(cheep.Timestamp, timestamp);
    }
    [Fact]
    public void GetRootEndpoint_ReturnsExpected()
    {
    
        var client = new HttpClient();
        var response = await client.GetAsync("/api/public");

        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
    
        Assert.Contains("Recent Cheeps", content);
        Assert.Contains("Cheep by @username", content);
    }

    [Fact]
    public void GetUserEndpoint_ReturnsExpected()
    {
        var client = new HttpClient();
        var response = await client.GetAsync("/api/private/username");

        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
    
        Assert.Contains("Your Cheeps", content);
        Assert.Contains("Cheep by @username", content);
    }
}