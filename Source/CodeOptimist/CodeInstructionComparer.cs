using System.Collections.Generic;
using HarmonyLib;

namespace CodeOptimist;

internal class CodeInstructionComparer : IEqualityComparer<CodeInstruction>
{
	public bool Equals(CodeInstruction x, CodeInstruction y)
	{
		if (x == null)
		{
			return false;
		}
		if (y == null)
		{
			return false;
		}
		if (x == y)
		{
			return true;
		}
		if (!object.Equals(x.opcode, y.opcode))
		{
			return false;
		}
		if (object.Equals(x.operand, y.operand))
		{
			return true;
		}
		if (x.operand == null || y.operand == null)
		{
			return true;
		}
		return false;
	}

	public int GetHashCode(CodeInstruction obj)
	{
		return obj.GetHashCode();
	}
}
