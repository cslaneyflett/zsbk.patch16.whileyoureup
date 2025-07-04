using System;

namespace CodeOptimist;

internal class Defer : IDisposable
{
	private readonly Action action;

	public Defer(Action action)
	{
		this.action = action;
	}

	public void Dispose()
	{
		action();
	}
}
