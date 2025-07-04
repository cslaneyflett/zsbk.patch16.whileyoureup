using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace CodeOptimist;

[HarmonyPatch]
internal abstract class Listing_TreeNonThingFilter : Listing_Tree
{
	private static readonly Dictionary<Type, Type> _subs = new Dictionary<Type, Type>
	{
		{
			typeof(Listing_TreeThingFilter),
			typeof(Listing_TreeNonThingFilter)
		},
		{
			typeof(ThingFilter),
			typeof(NonThingFilter)
		}
	};

	private static readonly Color NoMatchColor = Color.grey;

	private static readonly LRUCache<(TreeNode_ThingCategory, NonThingFilter), List<SpecialThingFilterDef>> cachedHiddenSpecialFilters = new LRUCache<(TreeNode_ThingCategory, NonThingFilter), List<SpecialThingFilterDef>>(500);

	private NonThingFilter filter;

	private NonThingFilter parentFilter;

	private const float IconSize = 20f;

	private const float IconOffset = 6f;

	private List<SpecialThingFilterDef> hiddenSpecialFilters;

	private List<ThingDef> forceHiddenDefs;

	private List<SpecialThingFilterDef> tempForceHiddenSpecialFilters;

	private List<ThingDef> suppressSmallVolumeTags;

	protected QuickSearchFilter searchFilter;

	public int matchCount;

	private Rect visibleRect;

	protected Listing_TreeNonThingFilter(NonThingFilter filter, NonThingFilter parentFilter, IEnumerable<ThingDef> forceHiddenDefs, IEnumerable<SpecialThingFilterDef> forceHiddenFilters, List<ThingDef> suppressSmallVolumeTags, QuickSearchFilter searchFilter)
	{
		_construct_Listing_TreeThingFilter(this, filter, parentFilter, forceHiddenDefs, forceHiddenFilters, suppressSmallVolumeTags, searchFilter);
	}

	[HarmonyPatch(typeof(Listing_TreeThingFilter), MethodType.Constructor, new Type[]
	{
		typeof(ThingFilter),
		typeof(ThingFilter),
		typeof(IEnumerable<ThingDef>),
		typeof(IEnumerable<SpecialThingFilterDef>),
		typeof(List<ThingDef>),
		typeof(QuickSearchFilter)
	})]
	[HarmonyReversePatch(HarmonyReversePatchType.Original)]
	private static void _construct_Listing_TreeThingFilter(object instance, NonThingFilter filter, NonThingFilter parentFilter, IEnumerable<ThingDef> forceHiddenDefs, IEnumerable<SpecialThingFilterDef> forceHiddenFilters, List<ThingDef> suppressSmallVolumeTags, QuickSearchFilter searchFilter)
	{
		Transpiler(null);
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codes)
		{
			return TranspilerHelper.ReplaceTypes(codes, _subs);
		}
	}

	[HarmonyPatch(typeof(Listing_TreeThingFilter), "ListCategoryChildren")]
	[HarmonyReversePatch(HarmonyReversePatchType.Original)]
	public void ListCategoryChildren(TreeNode_ThingCategory node, int openMask, Map map, Rect visibleRect)
	{
		Transpiler(null);
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codes)
		{
			return TranspilerHelper.ReplaceTypes(codes, _subs);
		}
	}

	[HarmonyPatch(typeof(Listing_TreeThingFilter), "DoCategoryChildren")]
	[HarmonyReversePatch(HarmonyReversePatchType.Original)]
	private void DoCategoryChildren(TreeNode_ThingCategory node, int indentLevel, int openMask, Map map, bool subtreeMatchedSearch)
	{
		Transpiler(null, null, null);
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> _codes, ILGenerator generator, MethodBase __originalMethod)
		{
			Transpiler transpiler = new Transpiler(_codes, __originalMethod);
			transpiler.codes = TranspilerHelper.ReplaceTypes(transpiler.codes, _subs);
			transpiler.TryInsertCodes(0, (int i, List<CodeInstruction> codes) => codes[i].Calls(AccessTools.DeclaredPropertyGetter(typeof(Find), "HiddenItemsManager")), (int i, List<CodeInstruction> codes) => new List<CodeInstruction>
			{
				new CodeInstruction(OpCodes.Br_S, codes[i + 3].operand)
			});
			return transpiler.GetFinalCodes();
		}
	}

	[HarmonyPatch(typeof(Listing_TreeThingFilter), "DoSpecialFilter")]
	[HarmonyReversePatch(HarmonyReversePatchType.Original)]
	private void DoSpecialFilter(SpecialThingFilterDef sfDef, int nestLevel)
	{
		Transpiler(null);
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codes)
		{
			return TranspilerHelper.ReplaceTypes(codes, _subs);
		}
	}

	[HarmonyPatch(typeof(Listing_TreeThingFilter), "DoCategory")]
	[HarmonyReversePatch(HarmonyReversePatchType.Original)]
	private void DoCategory(TreeNode_ThingCategory node, int indentLevel, int openMask, Map map, bool subtreeMatchedSearch)
	{
		Transpiler(null);
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codes)
		{
			return TranspilerHelper.ReplaceTypes(codes, _subs);
		}
	}

	[HarmonyPatch(typeof(Listing_TreeThingFilter), "DoThingDef")]
	[HarmonyReversePatch(HarmonyReversePatchType.Original)]
	private void DoThingDef(ThingDef tDef, int nestLevel, Map map)
	{
		Transpiler(null, null, null);
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> _codes, ILGenerator generator, MethodBase __originalMethod)
		{
			Transpiler transpiler = new Transpiler(_codes, __originalMethod);
			transpiler.codes = TranspilerHelper.ReplaceTypes(transpiler.codes, _subs);
			transpiler.TryInsertCodes(0, (int i, List<CodeInstruction> codes) => codes[i].Calls(AccessTools.DeclaredMethod(typeof(QuickSearchFilter), "Matches", new Type[1] { typeof(ThingDef) })), (int i, List<CodeInstruction> codes) => new List<CodeInstruction>
			{
				new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Def), "label")),
				new CodeInstruction(OpCodes.Call, AccessTools.DeclaredMethod(typeof(QuickSearchFilter), "Matches", new Type[1] { typeof(string) }))
			});
			transpiler.codes.RemoveRange(transpiler.MatchIdx, 1);
			return transpiler.GetFinalCodes();
		}
	}

	[HarmonyPatch(typeof(Listing_TreeThingFilter), "DoUndiscoveredEntry")]
	[HarmonyReversePatch(HarmonyReversePatchType.Original)]
	private void DoUndiscoveredEntry(int nestLevel, bool useIconOffset, List<ThingDef> toggledThingDefs)
	{
		Transpiler(null);
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codes)
		{
			return TranspilerHelper.ReplaceTypes(codes, _subs);
		}
	}

	[HarmonyPatch(typeof(Listing_TreeThingFilter), "AllowanceStateOf")]
	[HarmonyReversePatch(HarmonyReversePatchType.Original)]
	public MultiCheckboxState AllowanceStateOf(TreeNode_ThingCategory cat)
	{
		Transpiler(null);
		return MultiCheckboxState.On;
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codes)
		{
			return TranspilerHelper.ReplaceTypes(codes, _subs);
		}
	}

	[HarmonyPatch(typeof(Listing_TreeThingFilter), "Visible", new Type[] { typeof(ThingDef) })]
	[HarmonyReversePatch(HarmonyReversePatchType.Original)]
	private bool Visible(ThingDef td)
	{
		Transpiler(null);
		return false;
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codes)
		{
			return TranspilerHelper.ReplaceTypes(codes, _subs);
		}
	}

	[HarmonyPatch(typeof(Listing_TreeThingFilter), "IsOpen")]
	[HarmonyReversePatch(HarmonyReversePatchType.Original)]
	public override bool IsOpen(TreeNode node, int openMask)
	{
		Transpiler(null);
		return false;
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codes)
		{
			return TranspilerHelper.ReplaceTypes(codes, _subs);
		}
	}

	[HarmonyReversePatch(HarmonyReversePatchType.Original)]
	[HarmonyPatch(typeof(Listing_TreeThingFilter), "ThisOrDescendantsVisibleAndMatchesSearch")]
	private bool ThisOrDescendantsVisibleAndMatchesSearch(TreeNode_ThingCategory node)
	{
		Transpiler(null);
		return false;
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codes)
		{
			return TranspilerHelper.ReplaceTypes(codes, _subs);
		}
	}

	[HarmonyReversePatch(HarmonyReversePatchType.Original)]
	[HarmonyPatch(typeof(Listing_TreeThingFilter), "CategoryMatches")]
	private bool CategoryMatches(TreeNode_ThingCategory node)
	{
		Transpiler(null);
		return false;
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codes)
		{
			return TranspilerHelper.ReplaceTypes(codes, _subs);
		}
	}

	[HarmonyPatch(typeof(Listing_TreeThingFilter), "Visible", new Type[] { typeof(TreeNode_ThingCategory) })]
	[HarmonyReversePatch(HarmonyReversePatchType.Original)]
	private bool Visible(TreeNode_ThingCategory node)
	{
		Transpiler(null);
		return false;
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codes)
		{
			return TranspilerHelper.ReplaceTypes(codes, _subs);
		}
	}

	[HarmonyPatch(typeof(Listing_TreeThingFilter), "Visible", new Type[] { typeof(SpecialThingFilterDef) })]
	[HarmonyReversePatch(HarmonyReversePatchType.Original)]
	private bool Visible(SpecialThingFilterDef f)
	{
		Transpiler(null);
		return false;
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codes)
		{
			return TranspilerHelper.ReplaceTypes(codes, _subs);
		}
	}

	[HarmonyReversePatch(HarmonyReversePatchType.Original)]
	[HarmonyPatch(typeof(Listing_TreeThingFilter), "Visible", new Type[]
	{
		typeof(SpecialThingFilterDef),
		typeof(TreeNode_ThingCategory)
	})]
	private bool Visible(SpecialThingFilterDef filterDef, TreeNode_ThingCategory node)
	{
		Transpiler(null);
		return false;
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codes)
		{
			return TranspilerHelper.ReplaceTypes(codes, _subs);
		}
	}

	[HarmonyReversePatch(HarmonyReversePatchType.Original)]
	[HarmonyPatch(typeof(Listing_TreeThingFilter), "CurrentRowVisibleOnScreen")]
	private bool CurrentRowVisibleOnScreen()
	{
		Transpiler(null);
		return false;
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codes)
		{
			return TranspilerHelper.ReplaceTypes(codes, _subs);
		}
	}

	[HarmonyReversePatch(HarmonyReversePatchType.Original)]
	[HarmonyPatch(typeof(Listing_TreeThingFilter), "CalculateHiddenSpecialFilters", new Type[] { typeof(TreeNode_ThingCategory) })]
	private void CalculateHiddenSpecialFilters(TreeNode_ThingCategory node)
	{
		Transpiler(null);
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codes)
		{
			return TranspilerHelper.ReplaceTypes(codes, _subs);
		}
	}

	[HarmonyReversePatch(HarmonyReversePatchType.Original)]
	[HarmonyPatch(typeof(Listing_TreeThingFilter), "GetCachedHiddenSpecialFilters")]
	private static List<SpecialThingFilterDef> GetCachedHiddenSpecialFilters(TreeNode_ThingCategory node, NonThingFilter parentFilter)
	{
		Transpiler(null);
		return null;
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codes)
		{
			return TranspilerHelper.ReplaceTypes(codes, _subs);
		}
	}

	[HarmonyPatch(typeof(Listing_TreeThingFilter), "CalculateHiddenSpecialFilters", new Type[]
	{
		typeof(TreeNode_ThingCategory),
		typeof(ThingFilter)
	})]
	[HarmonyReversePatch(HarmonyReversePatchType.Original)]
	private static List<SpecialThingFilterDef> CalculateHiddenSpecialFilters(TreeNode_ThingCategory node, NonThingFilter parentFilter)
	{
		Transpiler(null);
		return null;
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codes)
		{
			return TranspilerHelper.ReplaceTypes(codes, _subs);
		}
	}

	[HarmonyReversePatch(HarmonyReversePatchType.Original)]
	[HarmonyPatch(typeof(Listing_TreeThingFilter), "ResetStaticData")]
	public static void ResetStaticData()
	{
		Transpiler(null);
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codes)
		{
			return TranspilerHelper.ReplaceTypes(codes, _subs);
		}
	}
}
