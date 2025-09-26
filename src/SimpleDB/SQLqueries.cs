namespace SimpleDB;

using System.Data.SQLite;

public static class SQLqueries {

    private static string extractTableNamesj 
        = "SELECT NAME from sqlite_master";

    // {0} = string | Table name
    // {1} = int | Limit
    private static string SelectLatestCheepsCommand 
        = "SELECT * FROM {0} ORDER BY timestamp DESC LIMIT {1};";

    
    public static SQLiteCommand SelectLatestCheeps(string table, int limit, SQLiteConnection conn) {
        return new SQLiteCommand(String.Format(SelectLatestCheepsCommand, table, limit), conn);
    }

}

public interface ISQLType<T>
{
    public static T Extract(SQLiteDataReader reader) 
        => throw new Exception("ISQlType<T>.Extract() Must be overriden");
}
