using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using Verse;

namespace CodeOptimist;

internal class Transpiler
{
	private class CodeNotFoundException : Exception
	{
		public CodeNotFoundException(List<CodeInstruction> sequence, MethodBase patchMethod, List<HarmonyLib.Patch> neighbors)
			: this("Unmatched sequence: " + string.Join(", ", sequence.Select(Extensions.ToIlString)), patchMethod, neighbors)
		{
		}

		private static MethodBase GetCallingMethod()
		{
			return (from frame in new StackTrace().GetFrames()
				select frame.GetMethod()).First((MethodBase method) => method.DeclaringType.Namespace != "CodeOptimist");
		}

		public CodeNotFoundException(MethodInfo matchMethod, MethodBase patchMethod, List<HarmonyLib.Patch> neighbors)
			: this("Unmatched predicate in calling method: " + GetCallingMethod().FullDescription(), patchMethod, neighbors)
		{
		}

		private CodeNotFoundException(string message, MethodBase patchMethod, List<HarmonyLib.Patch> neighbors)
			: base(message)
		{
			ModContentPack modContentPack = LoadedModManager.RunningModsListForReading.First((ModContentPack x) => x.assemblies.loadedAssemblies.Contains(patchMethod.DeclaringType?.Assembly));
			List<string> list = neighbors.Select((HarmonyLib.Patch n) => LoadedModManager.RunningModsListForReading.First((ModContentPack m) => m.assemblies.loadedAssemblies.Contains(n.PatchMethod.DeclaringType?.Assembly)).Name).Distinct().ToList();
			Log.Warning("[" + modContentPack.Name + "] You're welcome to 'Share logs' to my Discord: https://discord.gg/pnZGQAN \n");
			if (list.Any())
			{
				Log.Error("[" + modContentPack.Name + "] Likely conflict with one of: " + string.Join(", ", list));
			}
		}
	}

	public static readonly CodeInstructionComparer comparer = new CodeInstructionComparer();

	private readonly Dictionary<int, List<List<CodeInstruction>>> insertsAtIdxDict = new Dictionary<int, List<List<CodeInstruction>>>();

	private readonly List<HarmonyLib.Patch> neighbors = new List<HarmonyLib.Patch>();

	private readonly MethodBase originalMethod;

	private readonly MethodBase patchMethod;

	public List<CodeInstruction> codes;

	public int MatchIdx { get; private set; }

	public int InsertIdx { get; private set; }

	[MethodImpl(MethodImplOptions.NoInlining)]
	public Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase originalMethod)
	{
		this.originalMethod = originalMethod;
		patchMethod = new StackFrame(1).GetMethod();
		Patches patchInfo = Harmony.GetPatchInfo(originalMethod);
		if (patchInfo != null)
		{
			neighbors.AddRange(patchInfo.Transpilers);
		}
		codes = instructions.ToList();
	}

	public int TryFindCodeIndex(Predicate<CodeInstruction> match)
	{
		return TryFindCodeIndex(0, match);
	}

	public int TryFindCodeIndex(int startIndex, Predicate<CodeInstruction> match)
	{
		return TryFind(match, () => codes.FindIndex(startIndex, match));
	}

	public int TryFindCodeLastIndex(Predicate<CodeInstruction> match)
	{
		return TryFindCodeLastIndex(codes.Count - 1, match);
	}

	public int TryFindCodeLastIndex(int startIndex, Predicate<CodeInstruction> match)
	{
		return TryFind(match, () => codes.FindLastIndex(startIndex, match));
	}

	private int TryFind(Predicate<CodeInstruction> match, Func<int> resultFunc)
	{
		int num;
		try
		{
			num = resultFunc();
		}
		catch (Exception)
		{
			throw new CodeNotFoundException(match.Method, patchMethod, neighbors);
		}
		if (num == -1)
		{
			throw new CodeNotFoundException(match.Method, patchMethod, neighbors);
		}
		return num;
	}

	public bool TrySequenceEqual(int startIndex, List<CodeInstruction> sequence)
	{
		try
		{
			return codes.GetRange(startIndex, sequence.Count).SequenceEqual(sequence, comparer);
		}
		catch (Exception)
		{
			throw new CodeNotFoundException(sequence, patchMethod, neighbors);
		}
	}

	public int TryFindCodeSequence(List<CodeInstruction> sequence)
	{
		return TryFindCodeSequence(0, sequence);
	}

	public int TryFindCodeSequence(int startIndex, List<CodeInstruction> sequence)
	{
		try
		{
			if (sequence.Count > codes.Count)
			{
				throw new InvalidOperationException();
			}
			return Enumerable.Range(startIndex, codes.Count - sequence.Count + 1).First((int i) => codes.Skip(i).Take(sequence.Count).SequenceEqual(sequence, comparer));
		}
		catch (InvalidOperationException)
		{
			BeyondCompare(sequence);
			throw new CodeNotFoundException(sequence, patchMethod, neighbors);
		}
	}

	public void TryInsertCodes(int offset, Func<int, List<CodeInstruction>, bool> match, Func<int, List<CodeInstruction>, List<CodeInstruction>> newCodes, bool bringLabels = false)
	{
		for (int i = MatchIdx; i < codes.Count; i++)
		{
			if (match(i, codes))
			{
				InsertIdx = i + offset;
				if (!insertsAtIdxDict.TryGetValue(InsertIdx, out var value))
				{
					List<List<CodeInstruction>> list = (insertsAtIdxDict[InsertIdx] = new List<List<CodeInstruction>>());
					value = list;
				}
				List<CodeInstruction> list3 = newCodes(i, codes);
				value.Add(list3);
				if (bringLabels)
				{
					list3[0].labels.AddRange(codes[InsertIdx].labels);
					codes[InsertIdx].labels.Clear();
				}
				MatchIdx = i;
				return;
			}
		}
		throw new CodeNotFoundException(match.Method, patchMethod, neighbors);
	}

	public IEnumerable<CodeInstruction> GetFinalCodes(bool debug = false)
	{
		List<CodeInstruction> list = new List<CodeInstruction>();
		for (int i = 0; i < codes.Count; i++)
		{
			if (insertsAtIdxDict.TryGetValue(i, out var value))
			{
				foreach (List<CodeInstruction> item in value)
				{
					list.AddRange(item);
				}
			}
			list.Add(codes[i]);
		}
		if (debug)
		{
			BeyondCompare(list);
		}
		return list.AsEnumerable();
	}

	public void BeyondCompare(IEnumerable<CodeInstruction> outCodes)
	{
	}
}
