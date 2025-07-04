using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;

namespace CodeOptimist;

internal static class TranspilerHelper
{
	public static List<CodeInstruction> ReplaceTypes(IEnumerable<CodeInstruction> codes, Dictionary<Type, Type> subs)
	{
		List<CodeInstruction> list = (codes as List<CodeInstruction>) ?? codes.ToList();
		foreach (CodeInstruction item in list)
		{
			if (item.operand is MethodInfo methodInfo && subs.TryGetValue(methodInfo.DeclaringType, out var value))
			{
				Type[] parameters = (from x in methodInfo.GetParameters()
					select x.ParameterType).ToArray();
				Type[] genericArguments = methodInfo.GetGenericArguments();
				MethodInfo methodInfo2 = (methodInfo.IsGenericMethod ? AccessTools.DeclaredMethod(value, methodInfo.Name, parameters, genericArguments) : AccessTools.DeclaredMethod(value, methodInfo.Name, parameters));
				if (methodInfo2 != null)
				{
					item.operand = methodInfo2;
				}
			}
		}
		return list;
	}

	public static string NameWithType(this MethodBase method, bool withNamespace = true)
	{
		string text = method.DeclaringType.FullName;
		int num = text.IndexOf('[');
		if (num >= 0)
		{
			text = text.Substring(0, num);
		}
		return (withNamespace ? text : text.Substring(method.DeclaringType.Namespace.Length + 1)) + "." + method.Name;
	}
}
