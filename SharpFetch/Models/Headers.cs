using System.Collections;

namespace SharpFetch.Models
{
    public class Headers : IEnumerable<KeyValuePair<string, string>>
    {
        public static implicit operator Headers(Dictionary<string, string> dict) => new Headers(dict);

        // Ordinally ignore case for header keys, as per HTTP specifications.
        private readonly Dictionary<string, List<string>> _headers = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

        public Headers(Dictionary<string, string>? headers = null)
        {
            if (headers == null) return;
            foreach (var kvp in headers)
            {
                Set(kvp.Key, kvp.Value);
            }
        }

        public void Set(string key, string value)
        {
            _headers[key] = [value];
        }

        public void Append(string key, string value)
        {
            if (!_headers.TryGetValue(key, out var list))
                _headers[key] = list = [];

            list.Add(value);
        }

        public void Delete(string key)
        {
            _headers.Remove(key);
        }

        public string? Get(string key)
        {
            if (_headers.TryGetValue(key, out var values) && values.Count > 0)
            {
                return string.Join(", ", values);
            }
            return null;
        }

        public IReadOnlyList<string> GetAll(string key)
        {
            if (_headers.TryGetValue(key, out var values))
            {
                return values.AsReadOnly();
            }
            return [];
        }

        public bool Has(string key)
        {
            return _headers.ContainsKey(key);
        }

        public IReadOnlyCollection<string> Keys => _headers.Keys;

        /// <summary>Allows: headers["Authorization"] = "Bearer token"</summary>
        public string? this[string name]
        {
            get => Get(name);
            set
            {
                if (value is null) Delete(name);
                else Set(name, value);
            }
        }

        // IEnumerable so foreach works

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            foreach (var kvp in _headers)
                yield return new KeyValuePair<string, string>(kvp.Key, string.Join(", ", kvp.Value));
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
