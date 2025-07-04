using System;
using HarmonyLib;

namespace CodeOptimist;

[HarmonyPatch(typeof(WeakDictionary<object, object>), "CullKeys")]
internal static class WeakDictionary__Cull_Patch
{
	private struct Entry
	{
		public int hashCode;

		public int next;

		public WeakReference<object> key;

		public object value;
	}

	[HarmonyPrefix]
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
