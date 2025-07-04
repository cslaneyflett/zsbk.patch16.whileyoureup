using System;
using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace CodeOptimist;

[HarmonyPatch]
internal abstract class NonThingFilter : IExposable
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

	[Unsaved(false)]
	private Action settingsChangedCallback;

	[Unsaved(false)]
	private TreeNode_ThingCategory displayRootCategoryInt;

	[Unsaved(false)]
	private HashSet<ThingDef> allowedDefs = new HashSet<ThingDef>();

	[Unsaved(false)]
	private List<SpecialThingFilterDef> disallowedSpecialFilters = new List<SpecialThingFilterDef>();

	[Unsaved(false)]
	public List<SpecialThingFilterDef> hiddenSpecialFilters = new List<SpecialThingFilterDef>();

	private ThingCategoryDef overrideRootDef;

	private bool onlySpecialFilters;

	private FloatRange allowedHitPointsPercents = FloatRange.ZeroToOne;

	private FloatRange allowedMentalBreakChance = FloatRange.ZeroToOne;

	public bool allowedHitPointsConfigurable = true;

	private QualityRange allowedQualities = QualityRange.All;

	public bool allowedQualitiesConfigurable = true;

	[MustTranslate]
	public string customSummary;

	private List<ThingDef> thingDefs;

	[NoTranslate]
	private List<string> categories;

	[NoTranslate]
	private List<string> tradeTagsToAllow;

	[NoTranslate]
	private List<string> tradeTagsToDisallow;

	[NoTranslate]
	private List<string> thingSetMakerTagsToAllow;

	[NoTranslate]
	private List<string> thingSetMakerTagsToDisallow;

	[NoTranslate]
	private List<string> disallowedCategories;

	[NoTranslate]
	private List<string> specialFiltersToAllow;

	[NoTranslate]
	private List<string> specialFiltersToDisallow;

	private List<StuffCategoryDef> stuffCategoriesToAllow;

	private List<ThingDef> allowAllWhoCanMake;

	private FoodPreferability disallowWorsePreferability;

	private bool disallowInedibleByHuman;

	private bool disallowDoesntProduceMeat;

	private bool disallowNotEverStorable;

	private Type allowWithComp;

	private Type disallowWithComp;

	private float disallowCheaperThan = float.MinValue;

	private List<ThingDef> disallowedThingDefs;

	public TreeNode_ThingCategory OverrideRootNode => _get_OverrideRootNode(this);

	public TreeNode_ThingCategory RootNode => _get_RootNode(this);

	public bool OnlySpecialFilters => _get_OnlySpecialFilters(this);

	public string Summary => _get_Summary(this);

	public ThingRequest BestThingRequest => _get_BestThingRequest(this);

	public ThingDef AnyAllowedDef => _get_AnyAllowedDef(this);

	public IEnumerable<ThingDef> AllowedThingDefs => _get_AllowedThingDefs(this);

	private static IEnumerable<ThingDef> AllStorableThingDefs => _get_AllStorableThingDefs();

	public int AllowedDefCount => _get_AllowedDefCount(this);

	public FloatRange AllowedHitPointsPercents
	{
		get
		{
			return _get_AllowedHitPointsPercents(this);
		}
		set
		{
			_set_AllowedHitPointsPercents(this, value);
		}
	}

	public FloatRange AllowedMentalBreakChance
	{
		get
		{
			return _get_AllowedMentalBreakChance(this);
		}
		set
		{
			_set_AllowedMentalBreakChance(this, value);
		}
	}

	public QualityRange AllowedQualityLevels
	{
		get
		{
			return _get_AllowedQualityLevels(this);
		}
		set
		{
			_set_AllowedQualityLevels(this, value);
		}
	}

	public TreeNode_ThingCategory DisplayRootCategory
	{
		get
		{
			return _get_DisplayRootCategory(this);
		}
		set
		{
			_set_DisplayRootCategory(this, value);
		}
	}

	protected NonThingFilter()
	{
	}

	protected NonThingFilter(ThingCategoryDef overrideRootDef, bool onlySpecialFilters = false)
	{
		_construct_ThingFilter(this, overrideRootDef, onlySpecialFilters);
	}

	[HarmonyPatch(typeof(ThingFilter), MethodType.Constructor)]
	[HarmonyReversePatch(HarmonyReversePatchType.Original)]
	private static void _construct_ThingFilter(object instance, ThingCategoryDef overrideRootDef, bool onlySpecialFilters = false)
	{
		Transpiler(null);
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codes)
		{
			return TranspilerHelper.ReplaceTypes(codes, _subs);
		}
	}

	protected NonThingFilter(Action settingsChangedCallback)
	{
		_construct_ThingFilter(this, settingsChangedCallback);
	}

	[HarmonyPatch(typeof(ThingFilter), MethodType.Constructor)]
	[HarmonyReversePatch(HarmonyReversePatchType.Original)]
	private static void _construct_ThingFilter(object instance, Action settingsChangedCallback)
	{
		Transpiler(null);
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codes)
		{
			return TranspilerHelper.ReplaceTypes(codes, _subs);
		}
	}

	[HarmonyReversePatch(HarmonyReversePatchType.Original)]
	[HarmonyPatch(typeof(ThingFilter), "OverrideRootNode", MethodType.Getter)]
	private static TreeNode_ThingCategory _get_OverrideRootNode(object instance)
	{
		Transpiler(null);
		return null;
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codes)
		{
			return TranspilerHelper.ReplaceTypes(codes, _subs);
		}
	}

	[HarmonyReversePatch(HarmonyReversePatchType.Original)]
	[HarmonyPatch(typeof(ThingFilter), "RootNode", MethodType.Getter)]
	private static TreeNode_ThingCategory _get_RootNode(object instance)
	{
		Transpiler(null);
		return null;
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codes)
		{
			return TranspilerHelper.ReplaceTypes(codes, _subs);
		}
	}

	[HarmonyReversePatch(HarmonyReversePatchType.Original)]
	[HarmonyPatch(typeof(ThingFilter), "OnlySpecialFilters", MethodType.Getter)]
	private static bool _get_OnlySpecialFilters(object instance)
	{
		Transpiler(null);
		return false;
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codes)
		{
			return TranspilerHelper.ReplaceTypes(codes, _subs);
		}
	}

	[HarmonyReversePatch(HarmonyReversePatchType.Original)]
	[HarmonyPatch(typeof(ThingFilter), "Summary", MethodType.Getter)]
	private static string _get_Summary(object instance)
	{
		Transpiler(null);
		return null;
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codes)
		{
			return TranspilerHelper.ReplaceTypes(codes, _subs);
		}
	}

	[HarmonyReversePatch(HarmonyReversePatchType.Original)]
	[HarmonyPatch(typeof(ThingFilter), "BestThingRequest", MethodType.Getter)]
	private static ThingRequest _get_BestThingRequest(object instance)
	{
		Transpiler(null);
		return default(ThingRequest);
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codes)
		{
			return TranspilerHelper.ReplaceTypes(codes, _subs);
		}
	}

	[HarmonyPatch(typeof(ThingFilter), "AnyAllowedDef", MethodType.Getter)]
	[HarmonyReversePatch(HarmonyReversePatchType.Original)]
	private static ThingDef _get_AnyAllowedDef(object instance)
	{
		Transpiler(null);
		return null;
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codes)
		{
			return TranspilerHelper.ReplaceTypes(codes, _subs);
		}
	}

	[HarmonyReversePatch(HarmonyReversePatchType.Original)]
	[HarmonyPatch(typeof(ThingFilter), "AllowedThingDefs", MethodType.Getter)]
	private static IEnumerable<ThingDef> _get_AllowedThingDefs(object instance)
	{
		Transpiler(null);
		return null;
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codes)
		{
			return TranspilerHelper.ReplaceTypes(codes, _subs);
		}
	}

	[HarmonyReversePatch(HarmonyReversePatchType.Original)]
	[HarmonyPatch(typeof(ThingFilter), "AllStorableThingDefs", MethodType.Getter)]
	private static IEnumerable<ThingDef> _get_AllStorableThingDefs()
	{
		Transpiler(null);
		return null;
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codes)
		{
			return TranspilerHelper.ReplaceTypes(codes, _subs);
		}
	}

	[HarmonyReversePatch(HarmonyReversePatchType.Original)]
	[HarmonyPatch(typeof(ThingFilter), "AllowedDefCount", MethodType.Getter)]
	private static int _get_AllowedDefCount(object instance)
	{
		Transpiler(null);
		return 0;
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codes)
		{
			return TranspilerHelper.ReplaceTypes(codes, _subs);
		}
	}

	[HarmonyReversePatch(HarmonyReversePatchType.Original)]
	[HarmonyPatch(typeof(ThingFilter), "AllowedHitPointsPercents", MethodType.Getter)]
	private static FloatRange _get_AllowedHitPointsPercents(object instance)
	{
		Transpiler(null);
		return default(FloatRange);
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codes)
		{
			return TranspilerHelper.ReplaceTypes(codes, _subs);
		}
	}

	[HarmonyReversePatch(HarmonyReversePatchType.Original)]
	[HarmonyPatch(typeof(ThingFilter), "AllowedHitPointsPercents", MethodType.Setter)]
	private static void _set_AllowedHitPointsPercents(object instance, FloatRange value)
	{
		Transpiler(null);
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codes)
		{
			return TranspilerHelper.ReplaceTypes(codes, _subs);
		}
	}

	[HarmonyReversePatch(HarmonyReversePatchType.Original)]
	[HarmonyPatch(typeof(ThingFilter), "AllowedMentalBreakChance", MethodType.Getter)]
	private static FloatRange _get_AllowedMentalBreakChance(object instance)
	{
		Transpiler(null);
		return default(FloatRange);
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codes)
		{
			return TranspilerHelper.ReplaceTypes(codes, _subs);
		}
	}

	[HarmonyReversePatch(HarmonyReversePatchType.Original)]
	[HarmonyPatch(typeof(ThingFilter), "AllowedMentalBreakChance", MethodType.Setter)]
	private static void _set_AllowedMentalBreakChance(object instance, FloatRange value)
	{
		Transpiler(null);
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codes)
		{
			return TranspilerHelper.ReplaceTypes(codes, _subs);
		}
	}

	[HarmonyReversePatch(HarmonyReversePatchType.Original)]
	[HarmonyPatch(typeof(ThingFilter), "AllowedQualityLevels", MethodType.Getter)]
	private static QualityRange _get_AllowedQualityLevels(object instance)
	{
		Transpiler(null);
		return default(QualityRange);
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codes)
		{
			return TranspilerHelper.ReplaceTypes(codes, _subs);
		}
	}

	[HarmonyPatch(typeof(ThingFilter), "AllowedQualityLevels", MethodType.Setter)]
	[HarmonyReversePatch(HarmonyReversePatchType.Original)]
	private static void _set_AllowedQualityLevels(object instance, QualityRange value)
	{
		Transpiler(null);
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codes)
		{
			return TranspilerHelper.ReplaceTypes(codes, _subs);
		}
	}

	[HarmonyReversePatch(HarmonyReversePatchType.Original)]
	[HarmonyPatch(typeof(ThingFilter), "DisplayRootCategory", MethodType.Getter)]
	private static TreeNode_ThingCategory _get_DisplayRootCategory(object instance)
	{
		Transpiler(null);
		return null;
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codes)
		{
			return TranspilerHelper.ReplaceTypes(codes, _subs);
		}
	}

	[HarmonyReversePatch(HarmonyReversePatchType.Original)]
	[HarmonyPatch(typeof(ThingFilter), "DisplayRootCategory", MethodType.Setter)]
	private static void _set_DisplayRootCategory(object instance, TreeNode_ThingCategory value)
	{
		Transpiler(null);
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codes)
		{
			return TranspilerHelper.ReplaceTypes(codes, _subs);
		}
	}

	[HarmonyReversePatch(HarmonyReversePatchType.Original)]
	[HarmonyPatch(typeof(ThingFilter), "ExposeData")]
	public virtual void ExposeData()
	{
		Transpiler(null);
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codes)
		{
			return TranspilerHelper.ReplaceTypes(codes, _subs);
		}
	}

	[HarmonyReversePatch(HarmonyReversePatchType.Original)]
	[HarmonyPatch(typeof(ThingFilter), "ResolveReferences")]
	public void ResolveReferences()
	{
		Transpiler(null);
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codes)
		{
			return TranspilerHelper.ReplaceTypes(codes, _subs);
		}
	}

	[HarmonyReversePatch(HarmonyReversePatchType.Original)]
	[HarmonyPatch(typeof(ThingFilter), "RecalculateDisplayRootCategory")]
	public void RecalculateDisplayRootCategory()
	{
		Transpiler(null);
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codes)
		{
			return TranspilerHelper.ReplaceTypes(codes, _subs);
		}
	}

	[HarmonyReversePatch(HarmonyReversePatchType.Original)]
	[HarmonyPatch(typeof(ThingFilter), "RecalculateSpecialFilterConfigurability")]
	private void RecalculateSpecialFilterConfigurability()
	{
		Transpiler(null);
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codes)
		{
			return TranspilerHelper.ReplaceTypes(codes, _subs);
		}
	}

	[HarmonyReversePatch(HarmonyReversePatchType.Original)]
	[HarmonyPatch(typeof(ThingFilter), "IsAlwaysDisallowedDueToSpecialFilters")]
	public bool IsAlwaysDisallowedDueToSpecialFilters(ThingDef def)
	{
		Transpiler(null);
		return false;
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codes)
		{
			return TranspilerHelper.ReplaceTypes(codes, _subs);
		}
	}

	[HarmonyReversePatch(HarmonyReversePatchType.Original)]
	[HarmonyPatch(typeof(ThingFilter), "CopyAllowancesFrom")]
	public virtual void CopyAllowancesFrom(NonThingFilter other)
	{
		Transpiler(null);
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codes)
		{
			return TranspilerHelper.ReplaceTypes(codes, _subs);
		}
	}

	[HarmonyReversePatch(HarmonyReversePatchType.Original)]
	[HarmonyPatch(typeof(ThingFilter), "SetAllow", new Type[]
	{
		typeof(ThingDef),
		typeof(bool)
	})]
	public void SetAllow(ThingDef thingDef, bool allow)
	{
		Transpiler(null);
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codes)
		{
			return TranspilerHelper.ReplaceTypes(codes, _subs);
		}
	}

	[HarmonyReversePatch(HarmonyReversePatchType.Original)]
	[HarmonyPatch(typeof(ThingFilter), "SetAllow", new Type[]
	{
		typeof(SpecialThingFilterDef),
		typeof(bool)
	})]
	public void SetAllow(SpecialThingFilterDef sfDef, bool allow)
	{
		Transpiler(null);
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codes)
		{
			return TranspilerHelper.ReplaceTypes(codes, _subs);
		}
	}

	[HarmonyReversePatch(HarmonyReversePatchType.Original)]
	[HarmonyPatch(typeof(ThingFilter), "SetAllow", new Type[]
	{
		typeof(ThingCategoryDef),
		typeof(bool),
		typeof(IEnumerable<ThingDef>),
		typeof(IEnumerable<SpecialThingFilterDef>)
	})]
	public void SetAllow(ThingCategoryDef categoryDef, bool allow, IEnumerable<ThingDef> exceptedDefs = null, IEnumerable<SpecialThingFilterDef> exceptedFilters = null)
	{
		Transpiler(null);
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codes)
		{
			return TranspilerHelper.ReplaceTypes(codes, _subs);
		}
	}

	[HarmonyPatch(typeof(ThingFilter), "SetAllow", new Type[]
	{
		typeof(StuffCategoryDef),
		typeof(bool)
	})]
	[HarmonyReversePatch(HarmonyReversePatchType.Original)]
	public void SetAllow(StuffCategoryDef cat, bool allow)
	{
		Transpiler(null);
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codes)
		{
			return TranspilerHelper.ReplaceTypes(codes, _subs);
		}
	}

	[HarmonyReversePatch(HarmonyReversePatchType.Original)]
	[HarmonyPatch(typeof(ThingFilter), "SetAllowAllWhoCanMake")]
	public void SetAllowAllWhoCanMake(ThingDef thing)
	{
		Transpiler(null);
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codes)
		{
			return TranspilerHelper.ReplaceTypes(codes, _subs);
		}
	}

	[HarmonyReversePatch(HarmonyReversePatchType.Original)]
	[HarmonyPatch(typeof(ThingFilter), "SetFromPreset")]
	public void SetFromPreset(StorageSettingsPreset preset)
	{
		Transpiler(null);
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codes)
		{
			return TranspilerHelper.ReplaceTypes(codes, _subs);
		}
	}

	[HarmonyReversePatch(HarmonyReversePatchType.Original)]
	[HarmonyPatch(typeof(ThingFilter), "SetDisallowAll")]
	public void SetDisallowAll(IEnumerable<ThingDef> exceptedDefs = null, IEnumerable<SpecialThingFilterDef> exceptedFilters = null)
	{
		Transpiler(null);
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codes)
		{
			return TranspilerHelper.ReplaceTypes(codes, _subs);
		}
	}

	[HarmonyReversePatch(HarmonyReversePatchType.Original)]
	[HarmonyPatch(typeof(ThingFilter), "DisableSpecialFilters")]
	private void DisableSpecialFilters(TreeNode_ThingCategory category)
	{
		Transpiler(null);
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codes)
		{
			return TranspilerHelper.ReplaceTypes(codes, _subs);
		}
	}

	[HarmonyReversePatch(HarmonyReversePatchType.Original)]
	[HarmonyPatch(typeof(ThingFilter), "SetAllowAll")]
	public void SetAllowAll(NonThingFilter parentFilter, bool includeNonStorable = false)
	{
		Transpiler(null);
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codes)
		{
			return TranspilerHelper.ReplaceTypes(codes, _subs);
		}
	}

	[HarmonyPatch(typeof(ThingFilter), "Allows", new Type[] { typeof(Thing) })]
	[HarmonyReversePatch(HarmonyReversePatchType.Original)]
	public virtual bool Allows(Thing t)
	{
		Transpiler(null);
		return false;
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codes)
		{
			return TranspilerHelper.ReplaceTypes(codes, _subs);
		}
	}

	[HarmonyReversePatch(HarmonyReversePatchType.Original)]
	[HarmonyPatch(typeof(ThingFilter), "Allows", new Type[] { typeof(ThingDef) })]
	public bool Allows(ThingDef def)
	{
		Transpiler(null);
		return false;
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codes)
		{
			return TranspilerHelper.ReplaceTypes(codes, _subs);
		}
	}

	[HarmonyReversePatch(HarmonyReversePatchType.Original)]
	[HarmonyPatch(typeof(ThingFilter), "Allows", new Type[] { typeof(SpecialThingFilterDef) })]
	public bool Allows(SpecialThingFilterDef sf)
	{
		Transpiler(null);
		return false;
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codes)
		{
			return TranspilerHelper.ReplaceTypes(codes, _subs);
		}
	}

	[HarmonyReversePatch(HarmonyReversePatchType.Original)]
	[HarmonyPatch(typeof(ThingFilter), "GetThingRequest")]
	public ThingRequest GetThingRequest()
	{
		Transpiler(null);
		return default(ThingRequest);
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codes)
		{
			return TranspilerHelper.ReplaceTypes(codes, _subs);
		}
	}

	[HarmonyReversePatch(HarmonyReversePatchType.Original)]
	[HarmonyPatch(typeof(ThingFilter), "ToString")]
	public override string ToString()
	{
		Transpiler(null);
		return null;
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codes)
		{
			return TranspilerHelper.ReplaceTypes(codes, _subs);
		}
	}

	[HarmonyReversePatch(HarmonyReversePatchType.Original)]
	[HarmonyPatch(typeof(ThingFilter), "CreateOnlyEverStorableThingFilter")]
	public static NonThingFilter CreateOnlyEverStorableThingFilter()
	{
		Transpiler(null);
		return null;
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codes)
		{
			return TranspilerHelper.ReplaceTypes(codes, _subs);
		}
	}
}
