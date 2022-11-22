using System;
using System.Collections.Generic;

namespace Repository{
    public interface IRepository<T> where T : Identified{
        string Insert(T obj);
        void InsertRange(IEnumerable<T> objs);
        string Update(string key, Action<T> action);
        string Delete(string key);
        void DeleteRange(IEnumerable<string> keys);
        T Get(string key);
        long Count();
        IEnumerable<T> Get(Func<T, bool> query = null);
        IEnumerable<T> Get<TKey>(Func<T, bool> query = null, Func<T, TKey> orderBy = null);
        bool Any(Func<T, bool> query = null);
        void Clear();
    }
}