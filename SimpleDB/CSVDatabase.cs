namespace SimpleDB;

//using System.IO;

public sealed class CSVDatabase<T> : IDatabaseRepository<T>
{
    // Init database from CSV file
    public CSVDatabase(string filepath) 
    {
        throw new NotImplementedException();
//        if(!Path.Exists(filepath)) {
//
//        }
    }

    // Init empty databse
    public CSVDatabase()
    {
        throw new NotImplementedException();
    }

    // Add entry to databasee
    public void Store(T record)
    {
        throw new NotImplementedException();
    }

    // Return N latest entries
    public IEnumerable<T> Read(int limit)
    {
        throw new NotImplementedException();
    }

    // Return all entires
    public IEnumerable<T> Read()
    {
        return Read(Size());
    }

    // Write changes to file
    public void Write()
    {
        throw new NotImplementedException();
    }

    // Returns all queries that match lambda function condition 
    public IEnumerable<T> Query(Func<T, bool> condition)
    {
        throw new NotImplementedException();
    }

    // Number of entries in DB
    public int Size() 
    {
        throw new NotImplementedException();
    }

}