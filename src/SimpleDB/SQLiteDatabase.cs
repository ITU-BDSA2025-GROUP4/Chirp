namespace SimpleDB;

using System;
using System.Data;
using System.Data.SQLite;


public sealed class SQLiteDatabase<T> : IDatabaseRepository<T> where T : ISQLType<T>
{

    private SQLiteConnection conn;

    public SQLiteDatabase(string filepath)
    {
        if(!Path.Exists(filepath)) 
            throw new FileNotFoundException("Could not find: " + filepath);
        else
            filepath = Path.GetFullPath(filepath);

        conn = new SQLiteConnection("Data Source=" + filepath);

        conn.Open();
    }

    ~SQLiteDatabase() {
        conn.Close();
    }

    public IEnumerable<T> Read(int limit)
    {
        SQLiteCommand cmd = SQLqueries.SelectLatestCheeps("Cheep", limit, conn);

        List<T> results = new List<T>();

        using (SQLiteDataReader reader = cmd.ExecuteReader()) 
        {
            if(!reader.HasRows) {
                // Shit
            }

            while(reader.Read()) {
                results.Add(ISQLType<T>.Extract(reader));
            }
        }

        throw new NotImplementedException();
    }

    public IEnumerable<T> ReadAll()
    {
        return Read(Size());
    }

    public IEnumerable<T> Query(Func<T, bool> condition) 
    {
        return ReadAll().Where(condition);
    }

    public void Store(T record) {

    }

    public void Write() {
    }

    public int Size()
    {
        return 0;
    }

}