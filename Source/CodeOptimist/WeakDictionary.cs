using System;
using System.Collections.Generic;

namespace CodeOptimist;

internal class WeakDictionary<TKey, TValue> : Dictionary<WeakReference<TKey>, TValue> where TKey : class
{
	private class WeakReferenceEqualityComparer<T> : IEqualityComparer<WeakReference<T>> where T : class
	{
		public bool Equals(WeakReference<T> x, WeakReference<T> y)
		{
			if (x == null || y == null)
			{
				return false;
			}
			if (!x.TryGetTarget(out var target) || !y.TryGetTarget(out var target2))
			{
				return false;
			}
			return EqualityComparer<T>.Default.Equals(target, target2);
		}

		public int GetHashCode(WeakReference<T> obj)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj");
			}
			if (!obj.TryGetTarget(out var target))
			{
				return 0;
			}
			return EqualityComparer<T>.Default.GetHashCode(target);
		}
	}

	private static readonly WeakReferenceEqualityComparer<TKey> comparer = new WeakReferenceEqualityComparer<TKey>();

	private readonly WeakReference<TKey> temp = new WeakReference<TKey>(null);

	public TValue this[TKey key]
	{
		set
		{
			base[new WeakReference<TKey>(key)] = value;
		}
	}

	public WeakDictionary(int capacity = 0)
		: base(capacity, (IEqualityComparer<WeakReference<TKey>>)comparer)
	{
	}

	public void Add(TKey key, TValue value)
	{
		Add(new WeakReference<TKey>(key), value);
	}

	private WeakReference<TKey> GetTempWeakReference(TKey key)
	{
		temp.SetTarget(key);
		return temp;
	}

	public bool Remove(TKey key)
	{
		return Remove(GetTempWeakReference(key));
	}

	public bool TryGetValue(TKey key, out TValue value)
	{
		return TryGetValue(GetTempWeakReference(key), out value);
	}

	public TValue GetValueSafe(TKey key)
	{
		if (!TryGetValue(GetTempWeakReference(key), out var value))
		{
			return default(TValue);
		}
		return value;
	}

	private struct Entry
	{
		public int hashCode;

		public int next;

		public WeakReference<object> key;

		public object value;
	}

	private static void CullKeys(int[] ___buckets, Entry[] ___entries, ref int ___version, ref int ___freeList, ref int ___freeCount)
	{
		if (___entries == null)
		{
			return;
		}
		for (int i = 0; i < ___entries.Length; i++)
		{
			if (___entries[i].key == null || ___entries[i].key.TryGetTarget(out var _))
			{
				continue;
			}
			int num = ___entries[i].hashCode % ___buckets.Length;
			if (___buckets[num] == i)
			{
				___buckets[num] = ___entries[i].next;
			}
			else
			{
				for (int num2 = ___buckets[num]; num2 >= 0; num2 = ___entries[num2].next)
				{
					if (___entries[num2].next == i)
					{
						___entries[num2].next = ___entries[i].next;
						break;
					}
				}
			}
			___entries[i].hashCode = -1;
			___entries[i].next = ___freeList;
			___entries[i].key = null;
			___entries[i].value = null;
			___freeList = i;
			___freeCount++;
			___version++;
		}
	}
}
