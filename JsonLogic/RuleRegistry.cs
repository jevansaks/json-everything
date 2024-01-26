using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization.Metadata;
using Json.Logic.Rules;

namespace Json.Logic;

/// <summary>
/// Catalogs all of the known rules.
/// </summary>
public static class RuleRegistry
{
	private static readonly Dictionary<string, Type> _rules = new();
	private static readonly Dictionary<string, JsonTypeInfo> _ruleTypeInfos = new();

	internal static bool RequiresDynamicSerialization { get; private set; }
#if !NET6_0_OR_GREATER
		= true;
#endif

	private static List<string> GetOperators<T>()
	{
		return typeof(T).GetCustomAttributes<OperatorAttribute>().Select(a => a.Name).ToList();
	}

	static RuleRegistry()
	{
		var rulesData = new (Type, List<string>, JsonTypeInfo)[]
		{
			( typeof(AddRule), GetOperators<AddRule>(), JsonLogicSerializerContext.Default.AddRule ),
			( typeof(AllRule), GetOperators<AllRule>(), JsonLogicSerializerContext.Default.AllRule ),
			( typeof(AndRule), GetOperators<AndRule>(), JsonLogicSerializerContext.Default.AndRule ),
			( typeof(BooleanCastRule), GetOperators<BooleanCastRule>(), JsonLogicSerializerContext.Default.BooleanCastRule ),
			( typeof(CatRule), GetOperators<CatRule>(), JsonLogicSerializerContext.Default.CatRule ),
			( typeof(DivideRule), GetOperators<DivideRule>(), JsonLogicSerializerContext.Default.DivideRule ),
			( typeof(FilterRule), GetOperators<FilterRule>(), JsonLogicSerializerContext.Default.FilterRule ),
			( typeof(IfRule), GetOperators<IfRule>(), JsonLogicSerializerContext.Default.IfRule ),
			( typeof(IfRule), GetOperators<IfRule>(), JsonLogicSerializerContext.Default.IfRule ),
			( typeof(InRule), GetOperators<InRule>(), JsonLogicSerializerContext.Default.InRule ),
			( typeof(LessThanEqualRule), GetOperators<LessThanEqualRule>(), JsonLogicSerializerContext.Default.LessThanEqualRule ),
			( typeof(LessThanRule), GetOperators<LessThanRule>(), JsonLogicSerializerContext.Default.LessThanRule ),
			( typeof(LiteralRule), GetOperators<LiteralRule>(), JsonLogicSerializerContext.Default.LiteralRule ),
			( typeof(LogRule), GetOperators<LogRule>(), JsonLogicSerializerContext.Default.LogRule ),
			( typeof(LooseEqualsRule), GetOperators<LooseEqualsRule>(), JsonLogicSerializerContext.Default.LooseEqualsRule ),
			( typeof(LooseNotEqualsRule), GetOperators<LooseNotEqualsRule>(), JsonLogicSerializerContext.Default.LooseNotEqualsRule ),
			( typeof(MapRule), GetOperators<MapRule>(), JsonLogicSerializerContext.Default.MapRule ),
			( typeof(MaxRule), GetOperators<MaxRule>(), JsonLogicSerializerContext.Default.MaxRule ),
			( typeof(MergeRule), GetOperators<MergeRule>(), JsonLogicSerializerContext.Default.MergeRule ),
			( typeof(MinRule), GetOperators<MinRule>(), JsonLogicSerializerContext.Default.MinRule ),
			( typeof(MissingRule), GetOperators<MissingRule>(), JsonLogicSerializerContext.Default.MissingRule ),
			( typeof(MissingSomeRule), GetOperators<MissingSomeRule>(), JsonLogicSerializerContext.Default.MissingSomeRule ),
			( typeof(ModRule), GetOperators<ModRule>(), JsonLogicSerializerContext.Default.ModRule ),
			( typeof(MoreThanEqualRule), GetOperators<MoreThanEqualRule>(), JsonLogicSerializerContext.Default.MoreThanEqualRule ),
			( typeof(MoreThanRule), GetOperators<MoreThanRule>(), JsonLogicSerializerContext.Default.MoreThanRule ),
			( typeof(MultiplyRule), GetOperators<MultiplyRule>(), JsonLogicSerializerContext.Default.MultiplyRule ),
			( typeof(NoneRule), GetOperators<NoneRule>(), JsonLogicSerializerContext.Default.NoneRule ),
			( typeof(NotRule), GetOperators<NotRule>(), JsonLogicSerializerContext.Default.NotRule ),
			( typeof(OrRule), GetOperators<OrRule>(), JsonLogicSerializerContext.Default.OrRule ),
			( typeof(ReduceRule), GetOperators<ReduceRule>(), JsonLogicSerializerContext.Default.ReduceRule ),
			( typeof(SomeRule), GetOperators<SomeRule>(), JsonLogicSerializerContext.Default.SomeRule ),
			( typeof(StrictEqualsRule), GetOperators<StrictEqualsRule>(), JsonLogicSerializerContext.Default.StrictEqualsRule ),
			( typeof(StrictNotEqualsRule), GetOperators<StrictNotEqualsRule>(), JsonLogicSerializerContext.Default.StrictNotEqualsRule ),
			( typeof(SubstrRule), GetOperators<SubstrRule>(), JsonLogicSerializerContext.Default.SubstrRule ),
			( typeof(SubtractRule), GetOperators<SubtractRule>(), JsonLogicSerializerContext.Default.SubtractRule ),
			( typeof(VariableRule), GetOperators<VariableRule>(), JsonLogicSerializerContext.Default.VariableRule ),
		};
		foreach (var data in rulesData)
		{
			foreach (var op in data.Item2)
			{
				_rules[op] = data.Item1;
				_ruleTypeInfos[op] = data.Item3;
			}
		}
	}

	/// <summary>
	/// Gets a <see cref="Rule"/> implementation for a given identifier string.
	/// </summary>
	/// <param name="identifier">The identifier.</param>
	/// <returns>The <see cref="System.Type"/> of the rule.</returns>
	public static Type? GetRule(string identifier)
	{
		return _rules.TryGetValue(identifier, out var t) ? t : null;
	}

	/// <summary>
	/// Gets a <see cref="Rule"/> implementation for a given identifier string.
	/// </summary>
	/// <param name="identifier">The identifier.</param>
	/// <returns>The <see cref="System.Type"/> of the rule.</returns>
	internal static JsonTypeInfo? GetRuleTypeInfo(string identifier)
	{
		var result = _ruleTypeInfos.TryGetValue(identifier, out var t) ? t : null;
		if (result == null) Debug.Assert(RequiresDynamicSerialization, "If we didn't find the TypeInfo then RequiresDynamicSerialization should be true");
		return result;
	}

	/// <summary>
	/// Registers a new rule type.
	/// </summary>
	/// <typeparam name="T">The type of the rule to add.</typeparam>
	/// <remarks>
	/// Rules must contain a parameterless constructor.
	///
	/// Decorate your rule type with one or more <see cref="OperatorAttribute"/>s to
	/// define its identifier.
	///
	/// Registering a rule with an identifier that already exists will overwrite the
	/// existing registration.
	/// </remarks>
	[RequiresDynamicCode("For AOT support, use AddRule<T> that takes a JsonTypeInfo. Using this method requires reflection later.")]
	public static void AddRule<T>()
		where T : Rule
	{
		var type = typeof(T);
		var operators = GetOperators<T>();
		foreach (var name in operators)
		{
			_rules[name] = type;
		}
		RequiresDynamicSerialization = true;
	}

	/// <summary>
	/// Registers a new rule type.
	/// </summary>
	/// <typeparam name="T">The type of the rule to add.</typeparam>
	/// <remarks>
	/// Rules must contain a parameterless constructor.
	///
	/// Decorate your rule type with one or more <see cref="OperatorAttribute"/>s to
	/// define its identifier.
	///
	/// Registering a rule with an identifier that already exists will overwrite the
	/// existing registration.
	/// </remarks>
	[RequiresDynamicCode("For AOT support, use AddRule<T> that takes a JsonTypeInfo. Using this method requires reflection later.")]
	public static void AddRule<T>(JsonTypeInfo typeInfo)
		where T : Rule
	{
		var type = typeof(T);
		var operators = GetOperators<T>();
		foreach (var name in operators)
		{
			_rules[name] = type;
			_ruleTypeInfos[name] = typeInfo;
		}
	}
}