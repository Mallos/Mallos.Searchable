namespace Mallos.Searchable
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public sealed class SearchableCollection<T> : ICollection<IFilter<T>>
        where T : class
    {
        private readonly Dictionary<string, IFilter<T>> keyValues = new Dictionary<string, IFilter<T>>();

        public int Count => throw new NotImplementedException();

        public bool IsReadOnly => throw new NotImplementedException();

        public IFilter<T> FastFind(string key)
        {
            this.keyValues.TryGetValue(key, out var result);
            return result;
        }

        public void Add(IFilter<T> item)
        {
            if (this.keyValues.ContainsKey(item.Key))
                throw new IndexOutOfRangeException($"Key '{item.Key}' is already taken.");

            this.keyValues.Add(item.Key, item);
        }

        public bool Remove(IFilter<T> item)
        {
            // NOTE: Only check the key, not the object type.
            return this.keyValues.Remove(item.Key);
        }

        public void Clear()
        {
            this.keyValues.Clear();
        }

        public bool Contains(IFilter<T> item)
        {
            // NOTE: Only check the key, not the object type.
            return this.keyValues.ContainsKey(item.Key);
        }

        public void CopyTo(IFilter<T>[] array, int arrayIndex)
        {
            throw new NotSupportedException();
        }

        public IEnumerator<IFilter<T>> GetEnumerator()
        {
            return this.keyValues.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.keyValues.Values.GetEnumerator();
        }
    }
}
