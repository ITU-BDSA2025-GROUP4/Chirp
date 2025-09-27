namespace SimpleDB;

using Utils;
using System.Text;
using System.Data.SQLite;

/*
 * This abstracts SQL queries behind method calls. Please use this for all SQL
 * queries.
 *
 * Keep the queries defined in here very simple. If you need something complex,
 * then define a view in the DB and use a simple query to access it.
 *
 * Please following these guidelines when adding new SQL queries:
 *
 * When defining parameters always use the following variable names: @table,
 * @condition, @column, @limit
 *
 * If none of these fit your case, then add the variable name to the list above
 * and re-use it in the future for similar queries.
 *
 * If there are multiple tables, conditions, columns etc. then use @table1,
 * @table2, ..., @tableN.
 *
 * The name of the query string variable should always end with the posfix
 * "Str" and must be written in camelCase.
 *
 * The name of the SQLiteCommand object should always be the same as it's
 * string without the "Str" postfix. It must also be written in camelCase.
 *
 * The name of the associated method should be the same as the SQLiteCommand
 * object but written in PascalCase.
 *
 * Remember to add the query in the constructor and follow the pre-existing
 * pattern seen in the aforemnetioned constructor
 *
 */
public class SQLTableQueries {

    SQLiteConnection _conn;
    int _columns;

    // DEFINE QUERY STRINGS HERE
    private static readonly string selectTableNamesStr
        = "SELECT NAME FROM sqlite_master;";

    // {0} = string | Table name
    private static readonly string countTableEntriesStr
        = "SELECT count(*) FROM {0};";

    private static readonly string selectFromTableStr 
        = "SELECT * FROM {0} WHERE @condition;";

    private static readonly string selectFromTableWithLimitStr 
        = "SELECT * FROM {0} WHERE @condition LIMIT @limit;";

    private static readonly string insertIntoTableStr 
        = "INSERT INTO {0} VALUES({1});";

    // DEFINE SQLITECOMMANDS HERE
    private SQLiteCommand countTableEntries;
    private SQLiteCommand selectTableNames;
    private SQLiteCommand selectFromTable;
    private SQLiteCommand selectFromTableWithLimit;
    private SQLiteCommand insertIntoTable;

    public SQLTableQueries(SQLiteConnection conn, string table, int columns)
    {
        _conn = conn;
        _columns = columns;

        if(!IsQueryArgLegal(table))
        {
            throw new ArgumentException("Table argument contains illegal symbols");
        }

        countTableEntries = _conn.CreateCommand();
        countTableEntries.CommandText = String.Format(countTableEntriesStr, table);

        selectTableNames = _conn.CreateCommand();
        selectTableNames.CommandText = selectTableNamesStr;

        selectFromTable = _conn.CreateCommand();
        selectFromTable.CommandText = String.Format(selectFromTableStr, table);

        selectFromTableWithLimit = _conn.CreateCommand();
        selectFromTableWithLimit.CommandType = System.Data.CommandType.Text;
        selectFromTableWithLimit.CommandText = String.Format(selectFromTableWithLimitStr, table);

        insertIntoTable = _conn.CreateCommand();
        StringBuilder sb = new StringBuilder();
        // ** BEWARE **
        // WE ARE ASSUMING THAT EACH TABLE HAS AN ID COLUMN IN THE START
        for(int i = 0; i <= columns; i++)
        {
            sb.Append("@param" + i);
            if(i != columns) sb.Append(",");
        }
        insertIntoTable.CommandText = String.Format(insertIntoTableStr, table, sb.ToString());

        Optional<SQLiteDataReader> readerOpt = SelectTableNames();
        if(!readerOpt.HasValue) 
            throw new ArgumentException("Could not perform queries on give connection");

        SQLiteDataReader reader = readerOpt.Value();
        bool foundTable = false;
        while(reader.Read())
        {
            string tableName = reader.GetString(0);
            if(tableName == table)
            {
                foundTable = true;
                break;
            }
        }

        if(!foundTable)
        {
            throw new ArgumentException("Could not find table: " + table);
        }
    }

    // To check for characters that might be abused for SQL injections
    private static readonly char[] IllegalChars = 
    [
        '\'', ';'
    ];
    public static bool IsQueryArgLegal(string queryArg)
    {
        foreach(char ch in IllegalChars)
        {
            if(queryArg.Contains(ch)) return false;
        }
        if(queryArg.Contains("--")) return false;
        return true;
    }

    public Optional<SQLiteDataReader> CountTableEntries()
    {
        return Optional.Of(countTableEntries.ExecuteReader());
    }

    public Optional<SQLiteDataReader> SelectTableNames()
    {
        return Optional.Of(selectTableNames.ExecuteReader());
    }

    public Optional<SQLiteDataReader> SelectFromTable(string condition = "1=1")
    {
        if(!IsQueryArgLegal(condition)) 
            return Optional.Empty<SQLiteDataReader>();

        selectFromTable.Parameters.Clear();
        selectFromTable.Parameters.AddWithValue("@condition", condition);

        return Optional.Of(selectFromTable.ExecuteReader());
    }

    public Optional<SQLiteDataReader> SelectFromTableWithLimit(int limit, string condition = "1=1")
    {
        if(!IsQueryArgLegal(condition) || limit < 0) 
            return Optional.Empty<SQLiteDataReader>();

        selectFromTableWithLimit.Parameters.Clear();
        selectFromTableWithLimit.Parameters.Add("@limit", System.Data.DbType.Int32);
        selectFromTableWithLimit.Parameters["@limit"].Value = limit;

        selectFromTableWithLimit.Parameters.Add("@condition", System.Data.DbType.String);
        selectFromTableWithLimit.Parameters["@condition"].Value = condition;

        Console.WriteLine(selectFromTableWithLimit.CommandText);

        return Optional.Of(selectFromTableWithLimit.ExecuteReader());
    }

    public SQLiteCommand InsertIntoTable()
    {
        // Assuming first column is an id column
        // Setting to null means sqlit3 will simply use auto increment for the id
        insertIntoTable.Parameters.Clear();
        insertIntoTable.Parameters.Add("@param0", System.Data.DbType.Int32);
        insertIntoTable.Parameters["@param0"].Value = DBNull.Value;

        return insertIntoTable;
    }

}

public interface ISQLType<T>
{
    public static abstract T Extract(SQLiteDataReader reader);
    public void Insert(SQLiteParameterCollection parameters);
    public static abstract string ExpectedTable();
}