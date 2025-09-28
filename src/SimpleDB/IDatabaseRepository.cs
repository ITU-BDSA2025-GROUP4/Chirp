using System;
using System.Collections.Generic;

namespace SimpleDB;

public interface IDatabaseRepository<T>
{
    IEnumerable<T> Read(int limit);
    IEnumerable<T> ReadAll();
    IEnumerable<T> Query(Func<T, bool> condition);
    void Store(T record);
    void Write();
}