namespace SimpleDB;

// TODO: Check that given table exists in file

using Utils;
using System;
using System.Data;
using System.Data.SQLite;

public record Test(string author, string message, long timestamp) : ISQLType<Test>
{

    public static Test Extract(SQLiteDataReader reader)
    {
        int id = reader.GetInt32(0);
        string author = reader.GetString(1);
        string message = reader.GetString(2);
        long timestamp = reader.GetInt64(3);

        return new Test(author, message, timestamp);
    }

    public static string ExpectedTable()
    {
        return "Cheep";
    }

}

public sealed class SQLiteDatabase<T> : IDatabaseRepository<T> where T : ISQLType<T>
{

    private SQLiteConnection _conn;
    private string _table;

    public SQLiteDatabase(string filepath, string table)
    {
        if(!Path.Exists(filepath)) 
            throw new FileNotFoundException("Could not find: " + filepath);
        else
            filepath = Path.GetFullPath(filepath);

        _conn = new SQLiteConnection("Data Source=" + filepath);
        _table = table;

        _conn.Open();

        if(!DoesTableExist(table)) {
            throw new Exception("Welp");
        }

    }

    ~SQLiteDatabase()
    {
        _conn.Close();
    }

    public IEnumerable<T> Read(int limit)
    {
        SQLiteCommand cmd = SQLqueries.SelectLatestCheeps(_conn, limit);

        List<T> results = new List<T>();

        DataTable dt = new DataTable();
        dt.Load( cmd.ExecuteReader() );

        using (SQLiteDataReader reader = cmd.ExecuteReader()) 
        {
            if(reader.HasRows)
            while(reader.Read()) {
                
                results.Add(T.Extract(reader));
            }
        }

        return results;
    }

    public IEnumerable<T> ReadAll()
    {
        return Read(Size());
    }

    public IEnumerable<T> Query(Func<T, bool> condition)
    {
        return ReadAll().Where(condition);
    }

    public void Store(T record)
    {
        throw new NotImplementedException();
    }

    public void Write()
    {
        throw new NotImplementedException();
    }

    public int Size()
    {
        var cmd = SQLqueries.SelectCountTable(_conn, "Cheep");

        using (SQLiteDataReader reader = cmd.ExecuteReader()) 
        {
            if(!reader.HasRows) {
                return 0;
            }

            if(reader.Read())
            {
                return reader.GetInt32(0);
            }
        }

        return 0;
    }

    private bool DoesTableExist(string table) 
    {
        var cmd = SQLqueries.SelectTableNames(_conn);

        using (SQLiteDataReader reader = cmd.ExecuteReader()) 
        {
            if(!reader.HasRows) {
                return false;
            }
            while(reader.Read()) {
                string tableInDatabase = reader.GetString(0);
                if(tableInDatabase == table) return true;
            }
        }
        return false;
    }
}