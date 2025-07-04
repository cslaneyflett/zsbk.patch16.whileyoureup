using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Verse;

namespace CodeOptimist;

internal class Diagnostics
{
	private static readonly Harmony harmony = new Harmony("CodeOptimist.Diagnostics");

	private static readonly HashSet<string> openMethods = new HashSet<string>();

	private static readonly List<Assembly> loggedAssemblies = new List<Assembly>();

	private static readonly HashSet<MethodInfo> hasAssembliesLogged = new HashSet<MethodInfo>();

	private static bool logging;

	private static bool patched;

	private static int? realMethodCallStackIdx;

	private static readonly HashSet<string> ongoingLoggedMods = new HashSet<string>();

	public static void LogAssembly(Assembly assembly)
	{
		Type[] types = assembly.GetTypes();
		for (int i = 0; i < types.Length; i++)
		{
			LogType(types[i]);
		}
	}

	public static void LogNamespace(string @namespace)
	{
		foreach (Type item in from t in LoadedModManager.RunningModsListForReading.SelectMany((ModContentPack x) => x.assemblies.loadedAssemblies).SelectMany((Assembly x) => x.GetTypes())
			where t.IsClass && t.Namespace == @namespace
			select t)
		{
			LogType(item);
		}
	}

	public static void LogNamespace(string modName, string @namespace)
	{
		foreach (Type item in from t in LoadedModManager.RunningModsListForReading.FirstOrDefault((ModContentPack x) => x.Name == modName).assemblies.loadedAssemblies.SelectMany((Assembly x) => x.GetTypes())
			where t.IsClass
			where t.Namespace == @namespace || (t.Namespace?.StartsWith(@namespace + ".") ?? false)
			select t)
		{
			LogType(item);
		}
	}

	public static void LogType(Type type, List<string> excludedMethods = null, bool recursive = true)
	{
		foreach (MethodInfo item in AccessTools.GetDeclaredMethods(type).Where(delegate(MethodInfo x)
		{
			List<string> list = excludedMethods;
			return list == null || !list.Contains(x.Name);
		}))
		{
			LogMethod(item);
		}
		if (recursive)
		{
			Type[] nestedTypes = type.GetNestedTypes(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			for (int num = 0; num < nestedTypes.Length; num++)
			{
				LogType(nestedTypes[num], excludedMethods);
			}
		}
	}

	public static void LogMethod(MethodBase method)
	{
		try
		{
			harmony.Patch(method, new HarmonyMethod(AccessTools.Method(typeof(Diagnostics), "LogCall")));
		}
		catch
		{
		}
	}

	private static void LogCall(MethodInfo __originalMethod)
	{
		string[] array = Environment.StackTrace.Split(new char[1] { '\n' });
		openMethods.IntersectWith(array);
		int valueOrDefault = realMethodCallStackIdx.GetValueOrDefault();
		if (!realMethodCallStackIdx.HasValue)
		{
			valueOrDefault = array.Skip(1).FirstIndexOf((string s) => !s.StartsWith("  at CodeOptimist.Diagnostics")) + 1;
			realMethodCallStackIdx = valueOrDefault;
		}
		openMethods.Add(array[realMethodCallStackIdx.Value]);
		new string('\t', (openMethods.Count - 1) * 2);
	}

	public static void LogMods()
	{
		if (!patched)
		{
			foreach (MethodBase allPatchedMethod in Harmony.GetAllPatchedMethods())
			{
				try
				{
					harmony.Patch(allPatchedMethod, new HarmonyMethod(AccessTools.Method(typeof(Diagnostics), "LogMod")));
				}
				catch
				{
				}
			}
			patched = true;
		}
		hasAssembliesLogged.Clear();
		loggedAssemblies.Clear();
		logging = true;
	}

	private static void LogMod(MethodInfo __originalMethod)
	{
		if (!logging)
		{
			return;
		}
		LogCall(__originalMethod);
		if (hasAssembliesLogged.Contains(__originalMethod))
		{
			return;
		}
		Patches patchInfo = Harmony.GetPatchInfo(__originalMethod);
		foreach (HarmonyLib.Patch prefix in patchInfo.Prefixes)
		{
			loggedAssemblies.AddDistinct(prefix.PatchMethod.DeclaringType?.Assembly);
		}
		foreach (HarmonyLib.Patch postfix in patchInfo.Postfixes)
		{
			loggedAssemblies.AddDistinct(postfix.PatchMethod.DeclaringType?.Assembly);
		}
		foreach (HarmonyLib.Patch transpiler in patchInfo.Transpilers)
		{
			loggedAssemblies.AddDistinct(transpiler.PatchMethod.DeclaringType?.Assembly);
		}
		foreach (HarmonyLib.Patch finalizer in patchInfo.Finalizers)
		{
			loggedAssemblies.AddDistinct(finalizer.PatchMethod.DeclaringType?.Assembly);
		}
		hasAssembliesLogged.Add(__originalMethod);
	}

	public static void ReportOngoingMods()
	{
		logging = false;
		List<string> list = (from assembly in loggedAssemblies
			select LoadedModManager.RunningModsListForReading.First((ModContentPack modContent) => modContent.assemblies.loadedAssemblies.Contains(assembly)) into x
			select x.Name).ToList();
		list.Sort();
		if (ongoingLoggedMods.Count == 0)
		{
			ongoingLoggedMods.AddRange(list);
		}
		else
		{
			ongoingLoggedMods.IntersectWith(list);
		}
		foreach (string ongoingLoggedMod in ongoingLoggedMods)
		{
			_ = ongoingLoggedMod;
		}
	}
}
