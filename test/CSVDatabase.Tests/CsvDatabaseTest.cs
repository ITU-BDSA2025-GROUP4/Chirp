using System.Globalization;

using Chirp.Types;

using CsvHelper;
using CsvHelper.Configuration;

using SimpleDB;

using Xunit;

namespace CSVDatabase.Tests;

public class CsvDatabaseTest
{
    [Fact]
    public void Ctor_TextReader_LoadsAllRecords()
    {
        var rows = new[]
        {
            new Cheep("a1", "m1", 1), new Cheep("a2", "m2", 2), new Cheep("a3", "m3", 3),
        };

        using var r = new StringReader(ToCsvString(rows));
        var db = new CsvDatabase<Cheep>(r);

        Assert.Equal(3, db.Size());
        Assert.True(rows.All(x => db.ReadAll().Contains(x)));
    }

    [Fact]
    public void Store_IncreasesSize_AndAppearsInReadAll()
    {
        using var r = new StringReader(ToCsvString([]));
        var db = new CsvDatabase<Cheep>(r);

        var c = new Cheep("Ask", "Hello", 42);
        db.Store(c);

        Assert.Equal(1, db.Size());
        Assert.Contains(c, db.ReadAll());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(10)]
    public void Read_ReturnsFirstNEntries(int limit)
    {
        var rows = new[]
        {
            new Cheep("A", "M1", 1), new Cheep("B", "M2", 2), new Cheep("C", "M3", 3),
        };

        using var r = new StringReader(ToCsvString(rows));
        var db = new CsvDatabase<Cheep>(r);

        var result = db.Read(limit).ToList();
        Assert.Equal(Math.Min(limit, rows.Length), result.Count);
        Assert.True(result.SequenceEqual(rows.Take(result.Count)));
    }

    [Fact]
    public void Query_FiltersByPredicate()
    {
        var rows = new[]
        {
            new Cheep("Alice", "Hello", 1), new Cheep("Bob", "Hi", 2),
            new Cheep("Alice", "Again", 3),
        };

        using var r = new StringReader(ToCsvString(rows));
        var db = new CsvDatabase<Cheep>(r);

        var onlyAlice = db.Query(c => c.Author == "Alice").ToList();
        Assert.Equal(2, onlyAlice.Count);
        Assert.All(onlyAlice, c => Assert.Equal("Alice", c.Author));
    }

    [Fact]
    public void Write_NoPathCtor_DoesNotThrow()
    {
        using var r = new StringReader(ToCsvString([]));
        var db = new CsvDatabase<Cheep>(r);
        db.Store(new Cheep("A", "X", 1));

        var ex = Record.Exception(() => db.Write());
        Assert.Null(ex);
    }

    // helpers

    private static CsvConfiguration DefaultCsvConfig() =>
        new(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true, Delimiter = ",", MissingFieldFound = null
        };

    private static string ToCsvString(IEnumerable<Cheep> rows)
    {
        using var sw = new StringWriter();
        using var csv = new CsvWriter(sw, DefaultCsvConfig());
        csv.WriteHeader<Cheep>();
        csv.NextRecord();
        foreach (var r in rows)
        {
            csv.WriteRecord(r);
            csv.NextRecord();
        }

        return sw.ToString();
    }
}