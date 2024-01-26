﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Json.Logic.Rules;

namespace Json.Logic
{
	/// <summary>
	/// JsonSerializerContext for types in JsonLogic.
	/// </summary>
	[JsonSerializable(typeof(Rule[]))]
	[JsonSerializable(typeof(List<Rule>))]
	[JsonSerializable(typeof(JsonNode))]
	[JsonSerializable(typeof(AddRule))]
	[JsonSerializable(typeof(AllRule))]
	[JsonSerializable(typeof(AndRule))]
	[JsonSerializable(typeof(BooleanCastRule))]
	[JsonSerializable(typeof(CatRule))]
	[JsonSerializable(typeof(DivideRule))]
	[JsonSerializable(typeof(FilterRule))]
	[JsonSerializable(typeof(IfRule))]
	[JsonSerializable(typeof(IfRule))]
	[JsonSerializable(typeof(InRule))]
	[JsonSerializable(typeof(LessThanEqualRule))]
	[JsonSerializable(typeof(LessThanRule))]
	[JsonSerializable(typeof(LiteralRule))]
	[JsonSerializable(typeof(LogRule))]
	[JsonSerializable(typeof(LooseEqualsRule))]
	[JsonSerializable(typeof(LooseNotEqualsRule))]
	[JsonSerializable(typeof(MapRule))]
	[JsonSerializable(typeof(MaxRule))]
	[JsonSerializable(typeof(MergeRule))]
	[JsonSerializable(typeof(MinRule))]
	[JsonSerializable(typeof(MissingRule))]
	[JsonSerializable(typeof(MissingSomeRule))]
	[JsonSerializable(typeof(ModRule))]
	[JsonSerializable(typeof(MoreThanEqualRule))]
	[JsonSerializable(typeof(MoreThanRule))]
	[JsonSerializable(typeof(MultiplyRule))]
	[JsonSerializable(typeof(NoneRule))]
	[JsonSerializable(typeof(NotRule))]
	[JsonSerializable(typeof(OrRule))]
	[JsonSerializable(typeof(ReduceRule))]
	[JsonSerializable(typeof(SomeRule))]
	[JsonSerializable(typeof(StrictEqualsRule))]
	[JsonSerializable(typeof(StrictNotEqualsRule))]
	[JsonSerializable(typeof(SubstrRule))]
	[JsonSerializable(typeof(SubtractRule))]
	[JsonSerializable(typeof(VariableRule))]
	[JsonSerializable(typeof(RuleCollection))]
	public partial class JsonLogicSerializerContext : JsonSerializerContext
	{
	}
}
