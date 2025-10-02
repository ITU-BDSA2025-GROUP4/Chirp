namespace SimpleDB;

using Utils;
using System;
using System.Data;
using System.Data.SQLite;
using Microsoft.EntityFrameworkCore;

internal sealed class SQLTable<T> : DbContext where T : class
{
    private const string DefaultTable = "Table";

    internal string DbPath { get; }
    internal string TableName { get; }

    internal SQLTable(string path, string table)
    {
        DbPath = Path.GetFullPath(path);
        TableName = table;
        Database.EnsureCreated();
    }

    // From Microsoft docs
    // https://learn.microsoft.com/en-us/ef/core/get-started/overview/first-app?tabs=netcore-cli
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite($"Data Source={DbPath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.SharedTypeEntity<T>(DefaultTable).ToTable(TableName);
    }

    internal DbSet<T> Get()
    {
        return this.Set<T>(DefaultTable);
    }
}

public sealed class SQLiteDatabase<T> : IDatabaseRepository<T> where T : class
{
    private SQLTable<T> _table;
    private List<T> _unsavedEntries;
    private List<T> _buffer;

    internal SQLiteDatabase(string filepath)
    {
        if(!Path.Exists(filepath))
        {
            Directory.CreateDirectory(StringUtils.GetDirectoryPath(filepath));
            SQLiteConnection.CreateFile(filepath);
        }

        
        filepath = Path.GetFullPath(filepath);
        _table = new SQLTable<T>(filepath, typeof(T).Name);
        _table.Database.OpenConnection();

        _buffer = new List<T>(); 
        _unsavedEntries = new List<T>(); 
    }

    internal SQLiteDatabase() : this(StringUtils.UniqueFilePath("./logs/", "sql"))
    {}

    ~SQLiteDatabase()
    {
        var task = _table.SaveChanges();

        _table.Database.CloseConnection();
    }

    // If limit <= 0, then it returns everything
    public IEnumerable<T> Read(int limit)
    {
        _buffer.Clear();

        if(limit <= 0) 
        {
            _buffer.AddRange(_table.Get());
            _buffer.AddRange(_unsavedEntries);

            return _buffer;
        }

        _buffer.AddRange(_unsavedEntries.Take(limit));

        limit -= _buffer.Count();
        _buffer.AddRange(_table.Get().Take(limit));

        return _buffer;
    }

    public IEnumerable<T> ReadAll()
    {
        return Read(0);
    }

    public IEnumerable<T> Query(Func<T, bool> condition)
    {
        _buffer.Clear();

        _buffer.AddRange(
            _table.Get().Where(condition)
        );

        _buffer.AddRange(
            _unsavedEntries.Where(condition)
        );

        return _buffer;
    }

    public void Store(IEnumerable<T> record)
    {
        _table.Get().AddRange(record);
        _unsavedEntries.AddRange(record);
    }

    public void Store(T record)
    {
        _table.Get().Add(record);
        _unsavedEntries.Add(record);
    }

    public void Write()
    {
        _table.SaveChanges();
        _unsavedEntries.Clear();
    }

    public int Size()
    {
        return _table.Get().Count() + _unsavedEntries.Count();
    }
}