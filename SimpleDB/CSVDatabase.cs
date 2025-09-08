using System.Globalization;

namespace SimpleDB;

using System.Collections.Generic;

using CsvHelper;
using CsvHelper.Configuration;

public sealed class CsvDatabase<T> : IDatabaseRepository<T>
{
    private readonly string _path;
    private readonly CsvConfiguration _config;

    public CsvDatabase(string path, CsvConfiguration? config = null)
    {
        _path = Path.GetFullPath(path);
        _config = config ?? new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true, Delimiter = ",", MissingFieldFound = null
        };

        EnsureDirectoryExists(_path);
        EnsureHeaderExists();
    }

    // Init empty database
    public CsvDatabase()
        : this(Path.Combine("Data", $"{Guid.NewGuid()}.csv"))
    {
    }

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
        using var scope = new CsvWriteScope(_path, _config);
        EnsureHeaderIfNeeded(scope.Csv);
        scope.Csv.WriteRecord(record);
        scope.Csv.NextRecord();
    }

    // Return N latest entries
    public IEnumerable<T> Read(int limit)
    {
        using var scope = new CsvReadScope(_path, _config);
        var buffer = new Queue<T>(limit);

        while (scope.Csv.Read())
        {
            if (buffer.Count == limit)
            {
                buffer.Dequeue();
            }

            buffer.Enqueue(scope.Csv.GetRecord<T>());
        }

        return buffer.ToList();
    }

    // Return all entries
    public IEnumerable<T> ReadAll()
    {
        using var scope = new CsvReadScope(_path, _config);
        return scope.Csv.GetRecords<T>().ToList();
    }

    // Write changes to file
    public void Write()
    {
        throw new NotImplementedException();
    }

    // Returns all queries that match lambda function condition 
    public IEnumerable<T> Query(Func<T, bool> condition)
    {
        using var scope = new CsvReadScope(_path, _config);
        return scope.Csv.GetRecords<T>().Where(condition).ToList();
    }

    // Number of entries in DB
    public int Size()
    {
        using var scope = new CsvReadScope(_path, _config);

        if (_config.HasHeaderRecord)
        {
            if (!scope.Csv.Read()) return 0;
            scope.Csv.ReadHeader();
        }

        int count = 0;
        while (scope.Csv.Read()) count++;
        return count;
    }
}