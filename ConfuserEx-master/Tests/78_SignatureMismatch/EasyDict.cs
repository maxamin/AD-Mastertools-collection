using System.Collections;
using System.Collections.Generic;

namespace SignatureMismatch {
	public class EasyDict<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>> {
		private readonly Dictionary<TKey, TValue> _dict;

		public EasyDict() => _dict = new Dictionary<TKey, TValue>();

		public EasyDict(int capacity) => _dict = new Dictionary<TKey, TValue>(capacity);

		public int Count => _dict.Count;

		public IEnumerable<TValue> Values => _dict.Values;

		public TValue this[TKey key] {
			get => _dict.TryGetValue(key, out var value) ? value : default;
			set => _dict[key] = value;
		}

		public void Add(TKey key, TValue value) => _dict[key] = value;

		public void Clear() => _dict.Clear();

		public bool ContainsKey(TKey key) => _dict.ContainsKey(key);

		IEnumerator IEnumerable.GetEnumerator() => _dict.GetEnumerator();

		IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() => _dict.GetEnumerator();

		public bool Remove(TKey key) => _dict.Remove(key);

		public bool TryGetValue(TKey key, out TValue value) => _dict.TryGetValue(key, out value);
	}
}
