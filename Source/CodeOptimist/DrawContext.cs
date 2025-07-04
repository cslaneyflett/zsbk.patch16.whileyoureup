using System;
using UnityEngine;
using Verse;

namespace CodeOptimist;

internal class DrawContext : IDisposable
{
	private readonly Color guiColor;

	private readonly TextAnchor textAnchor;

	private readonly GameFont textFont;

	private readonly float labelPct;

	private readonly string keyPrefix;

	public Color GuiColor
	{
		set
		{
			GUI.color = value;
		}
	}

	public GameFont TextFont
	{
		set
		{
			Text.Font = value;
		}
	}

	public TextAnchor TextAnchor
	{
		set
		{
			Text.Anchor = value;
		}
	}

	public float LabelPct
	{
		set
		{
			Gui.labelPct = value;
		}
	}

	public string KeyPrefix
	{
		set
		{
			Gui.keyPrefix = value;
		}
	}

	public DrawContext()
	{
		guiColor = GUI.color;
		textFont = Text.Font;
		textAnchor = Text.Anchor;
		labelPct = Gui.labelPct;
		keyPrefix = Gui.keyPrefix;
	}

	public void Dispose()
	{
		GUI.color = guiColor;
		Text.Font = textFont;
		Text.Anchor = textAnchor;
		Gui.labelPct = labelPct;
		Gui.keyPrefix = keyPrefix;
	}
}
