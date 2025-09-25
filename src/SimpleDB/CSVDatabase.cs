using System.Globalization;

using CsvHelper;
using CsvHelper.Configuration;

namespace SimpleDB;

public sealed class CsvDatabase<T> : IDatabaseRepository<T>
{
    private readonly CsvConfiguration _config;
    private readonly string _path;
    private readonly List<T> _buffer;

    private readonly List<T> _entries;

    public CsvDatabase(string path, CsvConfiguration? config = null)
    {
        _path = Path.GetFullPath(path);
        _buffer = new List<T>();
        _config = config ?? new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true, Delimiter = ",", MissingFieldFound = null
        };


        EnsureDirectoryExists(_path);
        EnsureHeaderExists();


        // Initialize the entries of the database
        _entries = ReadAllFromFile();
    }

    // Init empty database
    public CsvDatabase() : this("./logs/tmp_db" + DateTimeOffset.Now.ToUnixTimeSeconds() + ".csv")
    {
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

        if (limit >= Size())
        {
            return _entries;
        }

        for (int i = 0; i < limit; i++)
        {
            _buffer.Add(_entries[i]);
        }

        return _buffer;
    }


    // Return all entries
    public IEnumerable<T> ReadAll()
    {
        return Read(Size());
    }

    // Write changes to file
    public void Write()
    {
        using CsvWriteScope scope = new(_path, _config, false);
        EnsureHeaderIfNeeded(scope.Csv);
        scope.Csv.WriteRecords(_entries);
    }

    // Returns all queries that match lambda function condition 
    public IEnumerable<T> Query(Func<T, bool> condition)
    {
        _buffer.Clear();

        foreach (T entry in _entries)
        {
            if (condition(entry))
            {
                _buffer.Add(entry);
            }
        }

        return _buffer;
    }

    private static void EnsureDirectoryExists(string path)
    {
        string? dir = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(dir))
        {
            Directory.CreateDirectory(dir);
        }
    }

    private void EnsureHeaderExists()
    {
        bool needHeader = !File.Exists(_path) || new FileInfo(_path).Length == 0;
        if (!needHeader || !_config.HasHeaderRecord)
        {
            return;
        }

        using CsvWriteScope scope = new(_path, _config);
        scope.Csv.WriteHeader<T>();
        scope.Csv.NextRecord();
    }

    //
    private void EnsureHeaderIfNeeded(CsvWriter csv)
    {
        if (!_config.HasHeaderRecord)
        {
            return;
        }

        bool needHeader = !File.Exists(_path) || new FileInfo(_path).Length == 0;
        if (!needHeader)
        {
            return;
        }

        csv.WriteHeader<T>();
        csv.NextRecord();
    }

    // Reads all entries from the database file and returns them contained in a list
    private List<T> ReadAllFromFile()
    {
        using CsvReadScope scope = new(_path, _config);
        return scope.Csv.GetRecords<T>().ToList();
    }

    // Number of entries in DB
    public int Size()
    {
        return _entries.Count();
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
}