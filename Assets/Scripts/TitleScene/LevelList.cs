using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[CreateAssetMenu(fileName = "LevelList", menuName = "StarterKit/Create Level List", order = 1)]
public class LevelList : ScriptableObject, IList<LevelItem>,
	IDictionary<string, LevelItem>,
	ISerializationCallbackReceiver
{
	public LevelItem[] levels;
	IDictionary<string, LevelItem> m_LevelDictionary;
	
	public int Count
	{
		get { return levels.Length; }
	}

	public bool IsReadOnly
	{
		get { return true; }
	}
	
	public LevelItem this[int i]
	{
		get { return levels[i]; }
	}

	public LevelItem this[string key]
	{
		get { return m_LevelDictionary[key]; }
	}

	public ICollection<string> Keys
	{
		get { return m_LevelDictionary.Keys; }
	}

	public int IndexOf(LevelItem item)
	{
		if (item == null)
		{
			return -1;
		}

		for (int i = 0; i < levels.Length; ++i)
		{
			if (levels[i] == item)
			{
				return i;
			}
		}

		return -1;
	}
	
	public bool Contains(LevelItem item)
	{
		return IndexOf(item) >= 0;
	}

	public bool ContainsKey(string key)
	{
		return m_LevelDictionary.ContainsKey(key);
	}

	public bool TryGetValue(string key, out LevelItem value)
	{
		return m_LevelDictionary.TryGetValue(key, out value);
	}

	public LevelItem GetLevelByScene(string scene)
	{
		for (int i = 0; i < levels.Length; ++i)
		{
			LevelItem item = levels[i];
			if (item != null &&
			    item.sceneName == scene)
			{
				return item;
			}
		}

		return null;
	}

	// Explicit interface implementations
	// Serialization listeners to create dictionary
	void ISerializationCallbackReceiver.OnBeforeSerialize()
	{
	}

	void ISerializationCallbackReceiver.OnAfterDeserialize()
	{
		m_LevelDictionary = levels.ToDictionary(l => l.id);
	}
	
	ICollection<LevelItem> IDictionary<string, LevelItem>.Values
	{
		get { return m_LevelDictionary.Values; }
	}

	LevelItem IList<LevelItem>.this[int i]
	{
		get { return levels[i]; }
		set { throw new NotSupportedException("Level List is read only"); }
	}

	LevelItem IDictionary<string, LevelItem>.this[string key]
	{
		get { return m_LevelDictionary[key]; }
		set { throw new NotSupportedException("Level List is read only"); }
	}

	void IList<LevelItem>.Insert(int index, LevelItem item)
	{
		throw new NotSupportedException("Level List is read only");
	}

	void IList<LevelItem>.RemoveAt(int index)
	{
		throw new NotSupportedException("Level List is read only");
	}

	void ICollection<LevelItem>.Add(LevelItem item)
	{
		throw new NotSupportedException("Level List is read only");
	}

	void ICollection<KeyValuePair<string, LevelItem>>.Add(KeyValuePair<string, LevelItem> item)
	{
		throw new NotSupportedException("Level List is read only");
	}

	void ICollection<KeyValuePair<string, LevelItem>>.Clear()
	{
		throw new NotSupportedException("Level List is read only");
	}

	bool ICollection<KeyValuePair<string, LevelItem>>.Contains(KeyValuePair<string, LevelItem> item)
	{
		return m_LevelDictionary.Contains(item);
	}

	void ICollection<KeyValuePair<string, LevelItem>>.CopyTo(KeyValuePair<string, LevelItem>[] array, int arrayIndex)
	{
		m_LevelDictionary.CopyTo(array, arrayIndex);
	}

	void ICollection<LevelItem>.Clear()
	{
		throw new NotSupportedException("Level List is read only");
	}

	void ICollection<LevelItem>.CopyTo(LevelItem[] array, int arrayIndex)
	{
		levels.CopyTo(array, arrayIndex);
	}

	bool ICollection<LevelItem>.Remove(LevelItem item)
	{
		throw new NotSupportedException("Level List is read only");
	}

	public IEnumerator<LevelItem> GetEnumerator()
	{
		return ((IList<LevelItem>) levels).GetEnumerator();
	}

	IEnumerator<KeyValuePair<string, LevelItem>> IEnumerable<KeyValuePair<string, LevelItem>>.GetEnumerator()
	{
		return m_LevelDictionary.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return levels.GetEnumerator();
	}

	void IDictionary<string, LevelItem>.Add(string key, LevelItem value)
	{
		throw new NotSupportedException("Level List is read only");
	}

	bool ICollection<KeyValuePair<string, LevelItem>>.Remove(KeyValuePair<string, LevelItem> item)
	{
		throw new NotSupportedException("Level List is read only");
	}

	bool IDictionary<string, LevelItem>.Remove(string key)
	{
		throw new NotSupportedException("Level List is read only");
	}

}