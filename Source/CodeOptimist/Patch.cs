using System.Runtime.CompilerServices;

namespace CodeOptimist;

internal static class Patch
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool Continue(object _ = null)
	{
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool Halt(object _ = null)
	{
		return false;
	}
}
