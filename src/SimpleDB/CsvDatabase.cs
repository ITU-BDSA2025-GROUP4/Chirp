using System.Globalization;
using System.Collections.Generic;
using CsvHelper;
using CsvHelper.Configuration;

namespace SimpleDB;

public sealed class CsvDatabase<T> : IDatabaseRepository<T>
{
    private readonly string _path;
    private readonly CsvConfiguration _config;
    private readonly List<T> _entries;
    private readonly List<T> _buffer = [];

    internal CsvDatabase(string path, CsvConfiguration? config = null)
    {
        _path = Path.GetFullPath(path);
        _config = config ?? CreateConfig();
        EnsureDirectoryExists(_path);
        EnsureHeaderExists();

        _entries = ReadAllFromFile();
    }

    internal CsvDatabase(TextReader reader, CsvConfiguration? config = null)
    {
        _path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".csv");
        _config = config ?? CreateConfig();
        using var csv = new CsvReader(reader, _config);
        _entries = csv.GetRecords<T>().ToList();
    }

    internal CsvDatabase() : this("./logs/tmp_db" + DateTimeOffset.Now.ToUnixTimeSeconds() + ".csv") {}

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

    private void EnsureHeaderIfNeeded(CsvWriter csv)
    {
        if (!_config.HasHeaderRecord) return;
        var needHeader = !File.Exists(_path) || new FileInfo(_path).Length == 0;
        if (!needHeader) return;
        csv.WriteHeader<T>();
        csv.NextRecord();
    }

    public void Store(T record) => _entries.Add(record);

    public IEnumerable<T> Read(int limit)
    {
        _buffer.Clear();
        _buffer.EnsureCapacity(limit);
        if (limit >= Size()) return _entries;
        for (int i = 0; i < limit; i++) _buffer.Add(_entries[i]);
        return _buffer;
    }

    private List<T> ReadAllFromFile()
    {
        using var scope = new CsvReadScope(_path, _config);
        return scope.Csv.GetRecords<T>().ToList();
    }

    public IEnumerable<T> ReadAll() => Read(Size());

    public void Write()
    {
        using var scope = new CsvWriteScope(_path, _config, false);
        EnsureHeaderIfNeeded(scope.Csv);
        scope.Csv.WriteRecords(_entries);
    }

    public IEnumerable<T> Query(Func<T, bool> condition)
    {
        _buffer.Clear();
        foreach (var e in _entries) if (condition(e)) _buffer.Add(e);
        return _buffer;
    }

    public int Size() => _entries.Count;

    private static CsvConfiguration CreateConfig() => new CsvConfiguration(CultureInfo.InvariantCulture)
    {
        HasHeaderRecord = true,
        Delimiter = ",",
        MissingFieldFound = null
    };
}