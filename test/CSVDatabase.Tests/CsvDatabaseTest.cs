using System.Globalization;

using Chirp.Types;

using CsvHelper;
using CsvHelper.Configuration;

using SimpleDB;

namespace CSVDatabase.Tests;

public class CsvDatabaseTest
{
    private readonly CsvDatabase<Cheep> _db;

    public CsvDatabaseTest()
    {
        _db = CreateTempDb();
    }


    [Fact]
    private void CreateInMemoryDb_TempDbMatchesDbFromFile()
    {
        Assert.True(_db.Equals(CreateTempDb()));
    }

    [Fact]
    private void AddEntryToDb_DbEntriesIncreaseByOne()
    {
        CsvDatabase<Cheep> db = CreateTempDb();
        db.Store(new Cheep("Ask", "Get me out", 1758806400));
        Assert.NotEqual(_db.Size(), db.Size());
    }

    private static string ToCsvString(IEnumerable<Cheep> rows)
    {
        using var sw = new StringWriter();
        var cfg = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true, Delimiter = ",", MissingFieldFound = null
        };
        using (var csv = new CsvWriter(sw, cfg))
        {
            csv.WriteHeader<Cheep>();
            csv.NextRecord();
            foreach (var r in rows)
            {
                csv.WriteRecord(r);
                csv.NextRecord();
            }
        }

        return sw.ToString();
    }

    private CsvDatabase<Cheep> CreateTempDb()
    {
        var csvText = ToCsvString(_db.ReadAll());
        using var reader = new StringReader(csvText);
        return new CsvDatabase<Cheep>(reader);
    }
}