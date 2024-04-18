using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableDictionary<TKey, TValue> : ISerializationCallbackReceiver, IDictionary<TKey, TValue>,
	ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>,
	IEnumerable, IDictionary, ICollection
{
	[System.NonSerialized]
	private Dictionary<TKey, TValue> dict;

	[SerializeField]
	private TKey[] keys;

	[SerializeField]
	private TValue[] values;

	public SerializableDictionary()
	{
		dict = new Dictionary<TKey, TValue>();
	}
	public SerializableDictionary(int capacity)
	{
		dict = new Dictionary<TKey, TValue>(capacity);
	}
	public SerializableDictionary(IDictionary<TKey, TValue> dictionary)
	{
		dict = new Dictionary<TKey, TValue>(dictionary);
	}

	#region ISerializationCallbackReceiver Interface
	void ISerializationCallbackReceiver.OnAfterDeserialize()
	{
		var tmpdict = new Dictionary<TKey, TValue>();
		Debug.Assert(keys == null && values == null || keys != null && values != null,
			"Dictionary inconsistency 발견. key 혹은 value 동시에 null이거나 null이지 않아야합니다.");
		if (keys == null && values == null)
		{
			dict = tmpdict;
			return;
		}

		Debug.Assert(keys != null && values != null && keys.Count() == values.Count(),
			"Dictionary inconsistency 발견. key 와 value 길이가 같아야합니다.");

		for (int i = 0; i < keys.Count(); i++)
		{
			tmpdict[keys[i]] = values[i];
		}
		dict = tmpdict;
		keys = null;
		values = null;
	}

	void ISerializationCallbackReceiver.OnBeforeSerialize()
	{
		keys = dict.Keys.ToArray();
		values = dict.Values.ToArray();
	}
	#endregion

	#region Dictionary<TKey, TValue> methods
	public void Add(TKey key, TValue value) { dict.Add(key, value); }
	public void Clear() { dict.Clear(); }
	public bool ContainsKey(TKey key) { return dict.ContainsKey(key); }
	public bool ContainsValue(TValue value) { return dict.ContainsValue(value); }
	public Dictionary<TKey, TValue>.Enumerator GetEnumerator() { return dict.GetEnumerator(); }
	public bool Remove(TKey key) { return dict.Remove(key); }
	public bool TryGetValue(TKey key, out TValue value) { return dict.TryGetValue(key, out value); }
	public IEqualityComparer<TKey> Comparer { get { return dict.Comparer; } }
	public int Count { get { return dict.Count; } }
	public TValue this[TKey key]
	{
		get { return dict[key]; }
		set { dict[key] = value; }
	}
	public Dictionary<TKey, TValue>.KeyCollection Keys { get { return dict.Keys; } }
	public Dictionary<TKey, TValue>.ValueCollection Values { get { return dict.Values; } }
	#endregion

	#region Explicit interface implements
	void ICollection.CopyTo(Array arr, int index) { ((ICollection)dict).CopyTo(arr, index); }
	bool ICollection.IsSynchronized { get { return ((ICollection)dict).IsSynchronized; } }
	object ICollection.SyncRoot { get { return ((ICollection)dict).SyncRoot; } }
	void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> pair)
	{ ((ICollection<KeyValuePair<TKey, TValue>>)dict).Add(pair); }
	bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> pair)
	{ return ((ICollection<KeyValuePair<TKey, TValue>>)dict).Contains(pair); }
	void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] arr, int index)
	{ ((ICollection<KeyValuePair<TKey, TValue>>)dict).CopyTo(arr, index); }
	bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
	{ get { return ((ICollection<KeyValuePair<TKey, TValue>>)dict).IsReadOnly; } }
	bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> pair)
	{ return ((ICollection<KeyValuePair<TKey, TValue>>)dict).Remove(pair); }
	void IDictionary.Add(object key, object value) { ((IDictionary)dict).Add(key, value); }
	bool IDictionary.Contains(object key) { return ((IDictionary)dict).Contains(key); }
	IDictionaryEnumerator IDictionary.GetEnumerator() { return ((IDictionary)dict).GetEnumerator(); }
	bool IDictionary.IsFixedSize { get { return ((IDictionary)dict).IsFixedSize; } }
	bool IDictionary.IsReadOnly { get { return ((IDictionary)dict).IsReadOnly; } }
	object IDictionary.this[object key]
	{
		get { return ((IDictionary)dict)[key]; }
		set { ((IDictionary)dict)[key] = value; }
	}
	ICollection IDictionary.Keys { get { return ((IDictionary)dict).Keys; } }
	ICollection IDictionary.Values { get { return ((IDictionary)dict).Values; } }
	void IDictionary.Remove(object key) { ((IDictionary)dict).Remove(key); }
	ICollection<TKey> IDictionary<TKey, TValue>.Keys
	{ get { return ((IDictionary<TKey, TValue>)dict).Keys; } }
	ICollection<TValue> IDictionary<TKey, TValue>.Values
	{ get { return ((IDictionary<TKey, TValue>)dict).Values; } }
	IEnumerator IEnumerable.GetEnumerator() { return ((IEnumerable)dict).GetEnumerator(); }
	IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
	{ return ((IEnumerable<KeyValuePair<TKey, TValue>>)dict).GetEnumerator(); }
	#endregion
}