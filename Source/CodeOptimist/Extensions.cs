using System;
using System.Runtime.CompilerServices;
using HarmonyLib;
using Verse;

namespace CodeOptimist;

internal static class Extensions
{
	public static string ModTranslate(this string key, params NamedArgument[] args)
	{
		return (Gui.keyPrefix + "_" + key).Translate(args).Resolve();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Squared(this float val)
	{
		return val * val;
	}

	public static string ToIlString(this CodeInstruction code)
	{
		object operand = code.operand;
		string text = ((operand is float) ? "f" : ((operand is double) ? "d" : ((operand is decimal) ? "m" : ((operand is uint) ? "u" : ((operand is long) ? "l" : ((!(operand is ulong)) ? "" : "ul"))))));
		string arg = text;
		return $"{code}{arg}";
	}

	public static string EllipsisTruncate(this string text, int len)
	{
		return text.Substring(0, Math.Min(len, text.Length)) + ((text.Length > len) ? "…" : "");
	}
}
