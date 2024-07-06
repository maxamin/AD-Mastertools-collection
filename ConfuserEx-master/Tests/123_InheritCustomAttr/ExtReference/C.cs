using System;
using System.Collections;
using System.Collections.Generic;

namespace InheritCustomAttr.ExtReference {
	class C<TKey, TValue> : IExt<TKey, TValue>, IDictionary<TKey, TValue>, IDictionary {
		public TValue this[TKey key] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public object this[object key] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public string Ext => throw new NotImplementedException();

		public int Count => throw new NotImplementedException();

		public ICollection<TKey> Keys => throw new NotImplementedException();

		public ICollection<TValue> Values => throw new NotImplementedException();

		public bool IsReadOnly => throw new NotImplementedException();

		public bool IsFixedSize => throw new NotImplementedException();

		public object SyncRoot => throw new NotImplementedException();

		public bool IsSynchronized => throw new NotImplementedException();

		ICollection IDictionary.Keys => throw new NotImplementedException();

		ICollection IDictionary.Values => throw new NotImplementedException();

		public void Add(TKey key, TValue value) => throw new NotImplementedException();

		public void Add(KeyValuePair<TKey, TValue> item) => throw new NotImplementedException();

		public void Add(object key, object value) => throw new NotImplementedException();

		public void Clear() => throw new NotImplementedException();

		public bool Contains(KeyValuePair<TKey, TValue> item) => throw new NotImplementedException();

		public bool Contains(object key) => throw new NotImplementedException();

		public bool ContainsKey(TKey key) => throw new NotImplementedException();

		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => throw new NotImplementedException();

		public void CopyTo(Array array, int index) => throw new NotImplementedException();

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => throw new NotImplementedException();

		public bool Remove(TKey key) => throw new NotImplementedException();

		public bool Remove(KeyValuePair<TKey, TValue> item) => throw new NotImplementedException();

		public void Remove(object key) => throw new NotImplementedException();

		public bool TryGetValue(TKey key, out TValue value) => throw new NotImplementedException();

		IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();

		IDictionaryEnumerator IDictionary.GetEnumerator() => throw new NotImplementedException();
	}
}
