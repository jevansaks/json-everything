﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Json.Logic.Rules;
using Json.More;

namespace Json.Logic;

/// <summary>
/// Catalogs all of the known rules.
/// </summary>
public static class RuleRegistry
{
	private static readonly ConcurrentDictionary<string, Type> _rules;
	private static readonly ConcurrentDictionary<Type, JsonSerializerContext> _externalRuleResolvers = new();

	// ReSharper disable once CoVariantArrayConversion
	internal static IJsonTypeInfoResolver[] ExternalTypeInfoResolvers => _externalRuleResolvers.Values.Distinct().ToArray();

	static RuleRegistry()
	{
		_rules = new ConcurrentDictionary<string, Type>(new Dictionary<string, Type>
		{
			{ "+", typeof(AddRule) },
			{ "all", typeof(AllRule) },
			{ "and", typeof(AndRule) },
			{ "!!", typeof(BooleanCastRule) },
			{ "cat", typeof(CatRule) },
			{ "/", typeof(DivideRule) },
			{ "filter", typeof(FilterRule) },
			{ "if", typeof(IfRule) },
			{ "?:", typeof(IfRule) },
			{ "in", typeof(InRule) },
			{ "<=", typeof(LessThanEqualRule) },
			{ "<", typeof(LessThanRule) },
			{ "", typeof(LiteralRule) },
			{ "log", typeof(LogRule) },
			{ "==", typeof(LooseEqualsRule) },
			{ "!=", typeof(LooseNotEqualsRule) },
			{ "map", typeof(MapRule) },
			{ "max", typeof(MaxRule) },
			{ "merge", typeof(MergeRule) },
			{ "min", typeof(MinRule) },
			{ "missing", typeof(MissingRule) },
			{ "missing_some", typeof(MissingSomeRule) },
			{ "%", typeof(ModRule) },
			{ ">=", typeof(MoreThanEqualRule) },
			{ ">", typeof(MoreThanRule) },
			{ "*", typeof(MultiplyRule) },
			{ "none", typeof(NoneRule) },
			{ "!", typeof(NotRule) },
			{ "or", typeof(OrRule) },
			{ "reduce", typeof(ReduceRule) },
			{ "some", typeof(SomeRule) },
			{ "===", typeof(StrictEqualsRule) },
			{ "!==", typeof(StrictNotEqualsRule) },
			{ "substr", typeof(SubstrRule) },
			{ "-", typeof(SubtractRule) },
			{ "var", typeof(VariableRule) }
		});
	}

	/// <summary>
	/// Gets a <see cref="Rule"/> implementation for a given identifier string.
	/// </summary>
	/// <param name="identifier">The identifier.</param>
	/// <returns>The <see cref="System.Type"/> of the rule.</returns>
	public static Type? GetRule(string identifier)
	{
		return _rules.GetValueOrDefault(identifier);
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
	[RequiresDynamicCode("For AOT support, use Register<T> that takes a JsonTypeInfo. Using this method requires reflection later.")]
	public static void AddRule<T>()
		where T : Rule
	{
		var type = typeof(T);
		var operators = type.GetCustomAttributes<OperatorAttribute>().Select(a => a.Name);
		foreach (var name in operators)
		{
			_rules[name] = type;
		}
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
	public static void AddRule<T>(JsonSerializerContext typeContext)
		where T : Rule
	{
		var type = typeof(T);
		var typeInfo = typeContext.GetTypeInfo(typeof(T)) ??
		               throw new ArgumentException($"Rule implementation `{typeof(T).Name}` does not have a JsonTypeInfo");
		_ = typeInfo.Converter as IJsonConverterReadWrite ??
		                throw new ArgumentException("Rule Converter must implement IJsonConverterReadWrite or AotCompatibleJsonConverter to be AOT compatible");
		var operators = type.GetCustomAttributes<OperatorAttribute>().Select(a => a.Name);
		foreach (var name in operators)
		{
			_rules[name] = type;
		}

		_externalRuleResolvers[type] = typeContext;

		LogicSerializerContext.ContextManager.NotifyTypeInfoResolverChanged();
	}
}