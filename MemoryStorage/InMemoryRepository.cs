using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace MemoryStorage{
    public class InMemoryRepository<T> : Repository.IRepository<T> where T : Repository.Identified
    {
        protected readonly ConcurrentDictionary<string, T> _cache;
        public InMemoryRepository(){
            _cache = new ConcurrentDictionary<string, T>();
        }
        public bool Any(Func<T, bool> query = null)
        {
            if(query == null)
                return _cache.Values.Any();
            return _cache.Values.Any(query);
        }

        public void Clear() => _cache.Clear();

        public long Count() => _cache.Count;

        public string Delete(string key)
        {
            if(!_cache.ContainsKey(key))
                throw new Repository.Exceptions.ResourceNotFoundException(key.ToString());
            _cache.TryRemove(key, out T value);
            return value.Key;
        }

        public void DeleteRange(IEnumerable<string> keys)
        {
            if (keys == null || !keys.Any())
                throw new ArgumentException($"null or [] is not a valid {nameof(keys)}", nameof(keys));
            var distinctKeys = keys.Distinct();
            if (keys.Count() != distinctKeys.Count())
                throw new Repository.Exceptions.UniqueKeyContraintException(keys.Except(distinctKeys).Select(x => x));
            var existingKeys = distinctKeys.Intersect(_cache.Keys);
            if (existingKeys.Count() != distinctKeys.Count())
                throw new Repository.Exceptions.ResourceNotFoundException(distinctKeys.Except(existingKeys).First());
            foreach (var key in keys)
                Delete(key);
        }

        public T Get(string key)
        {
            if (!_cache.ContainsKey(key))
                throw new Repository.Exceptions.ResourceNotFoundException(key.ToString());
            _cache.TryGetValue(key, out T value);
            return value;
        }

        public IEnumerable<T> Get(Func<T, bool> query = null)
        {
            if (query == null)
                return _cache.Values;
            return _cache.Values.Where(query);
        }

        public IEnumerable<T> Get<TKey>(Func<T, bool> query = null, Func<T, TKey> orderBy = null)
        {
            var list = Get(query);
            if (orderBy == null)
                return list;
            return list.OrderBy(orderBy);
        }

        public string Insert(T obj)
        {
            if(obj == null)
                throw new ArgumentException($"null is not a valid {nameof(obj)}", nameof(obj));
            if (_cache.ContainsKey(obj.Key))
                throw new Repository.Exceptions.UniqueKeyContraintException(new string[] {obj.Key.ToString()});
            if (!_cache.TryAdd(obj.Key, obj))
                throw new Repository.Exceptions.CRUDOperationException(Repository.CRUDOperation.Create);
            return obj.Key;
        }

        public void InsertRange(IEnumerable<T> objs)
        {
            if (objs == null || !objs.Any())
                throw new ArgumentException($"null or [] is not a valid {nameof(objs)}", nameof(objs));
            var keys = objs.Select(x => x.Key);
            var distinctKeys = objs.Select(x => x.Key).Distinct();
            if (keys.Count() != distinctKeys.Count())
                throw new Repository.Exceptions.UniqueKeyContraintException(keys.Except(distinctKeys).Select(x => x.ToString()));
            var existingKeys = distinctKeys.Intersect(_cache.Keys);
            if (existingKeys.Count() > 0)
                throw new Repository.Exceptions.UniqueKeyContraintException(existingKeys.Select(x => x.ToString()));
            foreach (var obj in objs)
                Insert(obj);
        }

        public string Update(string key, Action<T> action)
        {
            if (!_cache.ContainsKey(key))
                throw new Repository.Exceptions.ResourceNotFoundException(key.ToString());
            _cache.TryGetValue(key, out T value);
            action(value);
            return value.Key;
        }
    }
}