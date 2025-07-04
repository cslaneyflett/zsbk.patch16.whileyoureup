using System;
using HarmonyLib;
using Verse;

namespace CodeOptimist;

internal class NonThingFilter_LoadingContext : IDisposable
{
	private static class Log__Error_Patch
	{
		public static bool IgnoreCouldNotLoadReference(string text)
		{
			if (active && text.StartsWith("Could not load reference to "))
			{
				return Patch.Halt();
			}
			return Patch.Continue();
		}
	}

	private static readonly Harmony harmony = new Harmony("CodeOptimist.NonThingFilter");

	private static bool active;

	private static LoadSaveMode scribeMode;

	private readonly bool patched;

	public NonThingFilter_LoadingContext()
	{
		if (!patched)
		{
			harmony.Patch(AccessTools.Method(typeof(Log), "Error", new Type[1] { typeof(string) }), new HarmonyMethod(typeof(Log__Error_Patch), "IgnoreCouldNotLoadReference"));
			patched = true;
		}
		scribeMode = Scribe.mode;
		Scribe.mode = LoadSaveMode.LoadingVars;
		active = true;
	}

	public void Dispose()
	{
		active = false;
		Scribe.mode = scribeMode;
	}
}
