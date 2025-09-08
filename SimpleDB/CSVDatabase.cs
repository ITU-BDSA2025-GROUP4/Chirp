namespace SimpleDB;

using System.Collections.Generic;

using System.IO;
using CsvHelper;
using CsvHelper.Configuration;

public sealed class CSVDatabase<T> : IDatabaseRepository<T>
{
    private List<T> entries;
    private List<T> buffer;
    private string filepath;

    private CsvConfiguration csvconfig = new CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture)
    {
        HasHeaderRecord = true,
        Delimiter = ",",
        MissingFieldFound = null
    };

    // Init database from CSV file
    public CSVDatabase(string filepath) 
    {
        if(!Path.Exists(filepath)) 
        {
            throw new FileNotFoundException("File not found");
        }

        this.filepath = filepath;

        buffer = new List<T>();

        try
        {
            using (var reader = new StreamReader(filepath))
            using (var csv = new CsvReader(reader, csvconfig))
            {
                entries = csv.GetRecords<T>().ToList();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Failed to parse CSV file: " + e.Message);
        }
        finally {
            if(entries == null) entries = new List<T>();
        }

    }

    // Init empty databse
    public CSVDatabase()
    {
        entries = new List<T>();
        buffer = new List<T>();
        filepath = "default.db";
    }

    // Add entry to databasee
    public void Store(T record)
    {
        entries.Add(record);
    }

    // Return N latest entries
    public IEnumerable<T> Read(int limit)
    {
        buffer.Clear();
        buffer.EnsureCapacity(limit);

        int n = Size();
        if(limit >= n) { return entries;  }


        // Count = 3
        // Limit = 2
        //
        // [0,1,2]
        // Desired start index = 1
        for(int i = entries.Count() - limit; i < n; i++) {
            buffer.Add(entries[i]);
        }

        return buffer;
    }

    // Return all entires
    public IEnumerable<T> Read()
    {
        return Read(Size());
    }

    // Write changes to file
    public void Write()
    {
        using (var stream = new StreamWriter(this.filepath))
        {
            var csvConfig = new CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = stream.BaseStream.Length == 0,
                ShouldQuote = args => !string.IsNullOrEmpty(args.Field) && args.FieldType == typeof(string)

            };

            using (var csv = new CsvWriter(stream, csvConfig))
            {
                if (stream.BaseStream.Length == 0)
                {
                    csv.WriteHeader<T>();
                    csv.NextRecord();
                }

                for(int i = 0; i < Size(); i++) {
                    csv.WriteRecord(entries[i]);
                    csv.NextRecord();
                }
            }
        }
    }

    // Returns all queries that match lambda function condition 
    public IEnumerable<T> Query(Func<T, bool> condition)
    {
        buffer.Clear();
        int n = entries.Count();
        for(int i = 0; i < n; i++) {
            if(condition(entries[i]))  {
                buffer.Add(entries[i]);
            }
        }
        return buffer;
    }

    // Number of entries in DB
    public int Size() 
    {
        return entries.Count();
    }

}