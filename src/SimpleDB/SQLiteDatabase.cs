namespace SimpleDB;

// TODO: Check that given table exists in file

using Utils;
using System;
using System.Data.SQLite;

public sealed class SQLiteDatabase<T> : IDatabaseRepository<T> where T : ISQLType<T>
{

    private SQLiteConnection _conn;
    private SQLTableQueries _queries;

    private List<T> _buffer;

    public SQLiteDatabase(string filepath)
    {
        if(!Path.Exists(filepath)) 
            throw new FileNotFoundException("Could not find: " + filepath);
        else
            filepath = Path.GetFullPath(filepath);

        _conn = new SQLiteConnection("Data Source=" + filepath);

        _conn.Open();

        int numOfColumns = typeof(T).GetProperties().Length;
        _queries = new SQLTableQueries(_conn, T.ExpectedTable(), numOfColumns);

        _buffer = new List<T>(); 
    }

    ~SQLiteDatabase()
    {
        _conn.Close();
    }

    // If limit <= 0, then it returns everything
    public IEnumerable<T> Read(int limit)
    {
        _buffer.Clear();

        Optional<SQLiteDataReader> readerOpt; 
        if(limit > 0) readerOpt = _queries.SelectFromTableWithLimit(limit);
        else readerOpt = _queries.SelectFromTable();

        if(!readerOpt.HasValue)
        {
            Console.WriteLine("Query failed\n");
            return _buffer;
        }

        SQLiteDataReader reader = readerOpt.Value();
        if(!reader.HasRows)
        {
            Console.WriteLine("Query is empty\n");
            return _buffer;
        }

        while(reader.Read()) {
            _buffer.Add(T.Extract(reader));
        }

        reader.Close();
        return _buffer;
    }

    public IEnumerable<T> ReadAll()
    {
        return Read(0);
    }

    public IEnumerable<T> Query(Func<T, bool> condition)
    {
        throw new NotImplementedException();
    }

    public void Store(T record)
    {
        var cmd = _queries.InsertIntoTable();
        record.Insert(cmd.Parameters);
        cmd.ExecuteNonQuery();
    }

    public void Write()
    {
        throw new NotImplementedException();
    }

    public int Size()
    {
        Optional<SQLiteDataReader> cmd = _queries.CountTableEntries();

        if(!cmd.HasValue) return 0;

        using (SQLiteDataReader reader = cmd.Value()) 
        {
            if(!reader.HasRows)
                return 0;

            if(reader.Read())
                return reader.GetInt32(0);
        }

        return 0;
    }
}