using System.Globalization;

namespace SimpleDB;

using System.Collections.Generic;

using CsvHelper;
using CsvHelper.Configuration;

public sealed class CsvDatabase<T> : IDatabaseRepository<T>
{
    private readonly string _path;
    private readonly CsvConfiguration _config;

    private List<T> _entries;
    private List<T> _buffer;

    public CsvDatabase(string path, CsvConfiguration? config = null)
    {
        _path = Path.GetFullPath(path);
        _buffer = new List<T>();
        _config = config ?? new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            Delimiter = ",",
            MissingFieldFound = null
        };


        EnsureDirectoryExists(_path);
        EnsureHeaderExists();


        // Initialize the entries of the database
        _entries = ReadAllFromFile();
}

// Init empty database
public CsvDatabase() : this(Path.Combine(AppContext.BaseDirectory, "Resources", "Data", "chirp_cli_db.csv")) {}

    private static void EnsureDirectoryExists(string path)
    {
        var dir = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);
    }

    private void EnsureHeaderExists()
    {
        var needHeader = !File.Exists(_path) || new FileInfo(_path).Length == 0;
        if (!needHeader || !_config.HasHeaderRecord) return;

        using var scope = new CsvWriteScope(_path, _config, append: true);
        scope.Csv.WriteHeader<T>();
        scope.Csv.NextRecord();
    }

    //RAII style wrapper to handle resource management
    private readonly struct CsvReadScope : IDisposable
    {
        private StreamReader Stream { get; }
        public CsvReader Csv { get; }

        public CsvReadScope(string path, CsvConfiguration cfg)
        {
            Stream = new StreamReader(path);
            Csv = new CsvReader(Stream, cfg);
        }

        public void Dispose()
        {
            Csv.Dispose();
            Stream.Dispose();
        }
    }

    //RAII style wrapper to handle resource management
    private readonly struct CsvWriteScope : IDisposable
    {
        private StreamWriter Stream { get; }
        public CsvWriter Csv { get; }

        public CsvWriteScope(string path, CsvConfiguration cfg, bool append = true)
        {
            Stream = new StreamWriter(path, append);
            Csv = new CsvWriter(Stream, cfg);
        }

        public void Dispose()
        {
            Csv.Dispose();
            Stream.Dispose();
        }
    }

    //
    private void EnsureHeaderIfNeeded(CsvWriter csv)
    {
        if (!_config.HasHeaderRecord) return;
        var needHeader = !File.Exists(_path) || new FileInfo(_path).Length == 0;
        if (!needHeader) return;

        csv.WriteHeader<T>();
        csv.NextRecord();
    }

    // Add entry to databases
    public void Store(T record)
    {
        _entries.Add(record);
    }

    // Return N latest entries
    public IEnumerable<T> Read(int limit)
    {
        _buffer.Clear();
        _buffer.EnsureCapacity(limit);

        if (limit >= Size()) return _entries;

        for (int i = 0; i < limit; i++)
        {
            _buffer.Add(_entries[i]);
        }

        return _buffer;
    }

    // Reads all entries from the database file and returns them contained in a list
    private List<T> ReadAllFromFile()
    {
        using var scope = new CsvReadScope(_path, _config);
        return scope.Csv.GetRecords<T>().ToList();
    }


    // Return all entries
    public IEnumerable<T> ReadAll()
    {
        return Read(Size());
    }

    // Write changes to file
    public void Write()
    {
        using var scope = new CsvWriteScope(_path, _config, false);
        EnsureHeaderIfNeeded(scope.Csv);
        scope.Csv.WriteRecords(_entries);
    }

    // Returns all queries that match lambda function condition 
    public IEnumerable<T> Query(Func<T, bool> condition)
    {
        _buffer.Clear();

        foreach (var entry in _entries)
        {
            if (condition(entry)) _buffer.Add(entry);
        }

        return _buffer;
    }

    // Number of entries in DB
    public int Size()
    {
        return _entries.Count();
    }
}