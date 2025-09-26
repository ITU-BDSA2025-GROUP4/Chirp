namespace SimpleDB;

using System.Data.SQLite;

public static class SQLqueries {

    private static string selectTableNames
        = "SELECT NAME FROM sqlite_master;";

    // {0} = string | Table name
    private static string countTableEntries
        = "SELECT count(*) FROM {0};";

    // {0} = string | Table name
    // {1} = string | Order by
    // {2} = string | ASC or DESC
    // {3} = int | Limit
    private static string selectOrderByDescQuery 
        = "SELECT * FROM {0} ORDER BY {1} {2} LIMIT {3};";

    public static SQLiteCommand SelectCountTable(SQLiteConnection conn, string table)
    {
        return new SQLiteCommand(String.Format(countTableEntries, table), conn);
    }


    public static SQLiteCommand SelectTableNames(SQLiteConnection conn)
    {
        return new SQLiteCommand(selectTableNames, conn);
    }

    public static SQLiteCommand SelectLatestCheeps(SQLiteConnection conn, int limit) 
    {
        return new SQLiteCommand(String.Format(selectOrderByDescQuery, "Cheep", "timestamp", "DESC", limit), conn);
    }

}

public interface ISQLType<T>
{
    public static abstract T Extract(SQLiteDataReader reader);

    public static abstract string ExpectedTable();
}