using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace CodeOptimist;

internal static class Gui
{
	public const string symbols = "#\ufe0e *\ufe0e 0\ufe0e 1\ufe0e 2\ufe0e 3\ufe0e 4\ufe0e 5\ufe0e 6\ufe0e 7\ufe0e 8\ufe0e 9\ufe0e ©\ufe0e ®\ufe0e ‼\ufe0e ⁉\ufe0e ™\ufe0e ↔\ufe0e ↕\ufe0e ↖\ufe0e ↗\ufe0e ↘\ufe0e ↙\ufe0e ↩\ufe0e ↪\ufe0e Ⓜ\ufe0e ▪\ufe0e ▫\ufe0e ▶\ufe0e ◀\ufe0e ☀\ufe0e ☁\ufe0e ☂\ufe0e ☃\ufe0e ☄\ufe0e ☎\ufe0e ☑\ufe0e ☝\ufe0e ☠\ufe0e ☢\ufe0e ☣\ufe0e ☦\ufe0e ☪\ufe0e ☮\ufe0e ☯\ufe0e ☸\ufe0e ☹\ufe0e ☺\ufe0e ♀\ufe0e ♂\ufe0e ♈\ufe0e ♉\ufe0e ♊\ufe0e ♋\ufe0e ♌\ufe0e ♍\ufe0e ♎\ufe0e ♏\ufe0e ♐\ufe0e ♑\ufe0e ♒\ufe0e ♓\ufe0e ♟\ufe0e ♠\ufe0e ♣\ufe0e ♥\ufe0e ♦\ufe0e ♨\ufe0e ✂\ufe0e ✈\ufe0e ✉\ufe0e ✌\ufe0e ✍\ufe0e ✏\ufe0e ✒\ufe0e ✔\ufe0e ✖\ufe0e ✝\ufe0e ✡\ufe0e ✳\ufe0e ✴\ufe0e ❄\ufe0e ❇\ufe0e ❣\ufe0e ❤\ufe0e ➡\ufe0e ⤴\ufe0e ⤵\ufe0e 〰\ufe0e 〽\ufe0e ㊗\ufe0e ㊙\ufe0e";

	public static string keyPrefix;

	public static float labelPct = 0.5f;

	private static MultiCheckboxState paintingState;

	public static void DrawBool(this Listing_Standard list, ref bool value, string name)
	{
		list.CheckboxLabeled(("Label_" + name).ModTranslate(), ref value, ("Tooltip_" + name).ModTranslate());
	}

	private static void NumberLabel(this Listing_Standard list, Rect rect, float value, string format, string name, out string buffer)
	{
		Widgets.Label(new Rect(rect.x, rect.y, rect.width - 8f, rect.height), ("Label_" + name).ModTranslate());
		buffer = value.ToString(format);
		list.Gap(list.verticalSpacing);
		string text = ("Tooltip_" + name).ModTranslate();
		if (!text.NullOrEmpty())
		{
			if (Mouse.IsOver(rect))
			{
				Widgets.DrawHighlight(rect);
			}
			TooltipHandler.TipRegion(rect, text);
		}
	}

	public static void DrawFloat(this Listing_Standard list, ref float value, string name)
	{
		Rect rect = list.GetRect(Text.LineHeight);
		list.NumberLabel(rect.LeftPart(labelPct), value, "f1", name, out var buffer);
		Widgets.TextFieldNumeric(rect.RightPart(1f - labelPct), ref value, ref buffer, 0f, 999f);
	}

	public static void DrawPercent(this Listing_Standard list, ref float value, string name)
	{
		Rect rect = list.GetRect(Text.LineHeight);
		list.NumberLabel(rect.LeftPart(labelPct), value * 100f, "n0", name, out var buffer);
		Widgets.TextFieldPercent(rect.RightPart(1f - labelPct), ref value, ref buffer, 0f, 10f);
	}

	public static void DrawInt(this Listing_Standard list, ref int value, string name)
	{
		Rect rect = list.GetRect(Text.LineHeight);
		list.NumberLabel(rect.LeftPart(labelPct), value, "n0", name, out var buffer);
		Widgets.IntEntry(rect.RightPart(1f - labelPct), ref value, ref buffer);
	}

	public static void DrawEnum<T>(this Listing_Standard list, T value, string name, Action<T> setValue, float height = 30f)
	{
		DrawEnum(list.GetRect(height), value, name, setValue);
		list.Gap(list.verticalSpacing);
	}

	public static void DrawEnum<T>(Rect rect, T value, string name, Action<T> setValue)
	{
		string text = ("Tooltip_" + name).ModTranslate();
		if (!text.NullOrEmpty())
		{
			if (Mouse.IsOver(rect.LeftPart(labelPct)))
			{
				Widgets.DrawHighlight(rect);
			}
			TooltipHandler.TipRegion(rect.LeftPart(labelPct), text);
		}
		Widgets.Label(rect.LeftPart(labelPct), ("Label_" + name).ModTranslate());
		string name2 = Enum.GetName(typeof(T), value);
		if (!Widgets.ButtonText(rect.RightPart(1f - labelPct), ("Label_" + name + "_" + name2).ModTranslate()))
		{
			return;
		}
		List<FloatMenuOption> list = new List<FloatMenuOption>();
		foreach (T enumValue in Enum.GetValues(typeof(T)).Cast<T>())
		{
			string name3 = Enum.GetName(typeof(T), enumValue);
			list.Add(new FloatMenuOption(("Label_" + name + "_" + name3).ModTranslate(), delegate
			{
				setValue(enumValue);
			}));
		}
		Find.WindowStack.Add(new FloatMenu(list.ToList()));
	}

	public static void PaintableCheckboxMultiLabeled(Rect rect, string label, ref MultiCheckboxState state)
	{
		using (new DrawContext
		{
			TextAnchor = TextAnchor.MiddleLeft
		})
		{
			Rect rect2 = rect;
			rect2.xMax -= 24f;
			Widgets.Label(rect2, label);
			float x = (float)((double)rect.x + (double)rect.width - 24.0);
			float y = rect.y + (float)(((double)rect.height - 24.0) / 2.0);
			state = PaintableCheckboxMulti(new Rect(x, y, 24f, 24f), state);
		}
	}

	private static MultiCheckboxState PaintableCheckboxMulti(Rect rect, MultiCheckboxState state)
	{
		MouseoverSounds.DoRegion(rect);
		MultiCheckboxState multiCheckboxState = state;
		Widgets.DraggableResult draggableResult = Widgets.ButtonImageDraggable(rect, multiCheckboxState switch
		{
			MultiCheckboxState.On => Widgets.CheckboxOnTex, 
			MultiCheckboxState.Partial => Widgets.CheckboxPartialTex, 
			_ => Widgets.CheckboxOffTex, 
		});
		if (draggableResult is Widgets.DraggableResult.Dragged or Widgets.DraggableResult.Pressed or Widgets.DraggableResult.DraggedThenPressed)
		{
			multiCheckboxState = state switch
			{
				MultiCheckboxState.On => MultiCheckboxState.Partial, 
				MultiCheckboxState.Partial => MultiCheckboxState.Off, 
				_ => MultiCheckboxState.On, 
			};
		}

		var widgetsTraverse = Traverse.Create(typeof(Widgets));
		if (draggableResult == Widgets.DraggableResult.Dragged)
		{
			widgetsTraverse.Field("checkboxPainting")
				.SetValue(true);
			widgetsTraverse.Field("checkboxPaintingState")
				.SetValue(multiCheckboxState is MultiCheckboxState.On or MultiCheckboxState.Partial);
			paintingState = multiCheckboxState;
		}
		else if (widgetsTraverse.Field<bool>("checkboxPainting").Value && Mouse.IsOver(rect))
		{
			multiCheckboxState = paintingState;
		}
		if (multiCheckboxState != state)
		{
			SoundDef soundDef = ((multiCheckboxState != MultiCheckboxState.On) ? SoundDefOf.Checkbox_TurnedOff : SoundDefOf.Checkbox_TurnedOn);
			soundDef.PlayOneShotOnCamera();
		}
		return multiCheckboxState;
	}
}
