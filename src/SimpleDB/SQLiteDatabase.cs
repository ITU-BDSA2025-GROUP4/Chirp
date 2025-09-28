namespace SimpleDB;

// TODO: Check that given table exists in file

using Utils;
using System;
using System.Data;
using System.Data.SQLite;
using Microsoft.EntityFrameworkCore;

public interface ISQLType<T>
{
    public static abstract string TableName();
}

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

    // The following configures EF to create a Sqlite database file in the
    // special "local" folder for your platform.
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

public sealed class SQLiteDatabase<T> : IDatabaseRepository<T> where T : class, ISQLType<T>
{
    private SQLTable<T> _table;
    private List<T> _buffer;

    public SQLiteDatabase(string filepath)
    {
        if(!Path.Exists(filepath))
        {
            Directory.CreateDirectory(StringUtils.GetDirectoryPath(filepath));
            SQLiteConnection.CreateFile(filepath);
        }

        filepath = Path.GetFullPath(filepath);
        _table = new SQLTable<T>(filepath, T.TableName());
        _table.Database.OpenConnection();

        _buffer = new List<T>(); 
    }

    public SQLiteDatabase() : this( StringUtils.UniqueFilePath("./logs/", "sql")  )
    {}

    ~SQLiteDatabase()
    {
        var task = _table.SaveChangesAsync();
        task.Wait();

        _table.Database.CloseConnection();
    }

    // If limit <= 0, then it returns everything
    public IEnumerable<T> Read(int limit)
    {
        _buffer.Clear();

        _buffer.AddRange(
                limit > 0 ? 
                _table.Get().Take(limit)
                :
                _table.Get()
        );

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

        return _buffer;
    }

    public Task StoreAsync(T record)
    {
        _table.Get().Add(record);
        return _table.SaveChangesAsync();
    }

    public void StoreWithoutWriting(T record)
    {
        _table.Get().Add(record);
    }

    public void Store(IEnumerable<T> record)
    {
        _table.Get().AddRange(record);
        var task = _table.SaveChangesAsync();
        task.Wait();
    }

    public void Store(T record)
    {
        _table.Get().Add(record);
        var task = _table.SaveChangesAsync();
        task.Wait();
    }

    public Task WriteAsync()
    {
        return _table.SaveChangesAsync();
    }

    public void Write()
    {
        var task = _table.SaveChangesAsync();
        task.Wait();
    }

    public int Size()
    {
        return _table.Get().Count();
    }
}

