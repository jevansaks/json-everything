﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Json.More;

namespace Json.Logic.Rules;

/// <summary>
/// Handles the `merge` operation.
/// </summary>
[Operator("merge")]
[JsonConverter(typeof(MergeRuleJsonConverter))]
public class MergeRule : Rule
{
	/// <summary>
	/// A sequence of arrays to merge into a single array.
	/// </summary>
	protected internal List<Rule> Items { get; }

	/// <summary>
	/// Creates a new instance of <see cref="MergeRule"/> when 'merge' operator is detected within json logic.
	/// </summary>
	/// <param name="items">A sequence of arrays to merge into a single array.</param>
	protected internal MergeRule(params Rule[] items)
	{
		Items = items.ToList();
	}

	/// <summary>
	/// Applies the rule to the input data.
	/// </summary>
	/// <param name="data">The input data.</param>
	/// <param name="contextData">
	///     Optional secondary data.  Used by a few operators to pass a secondary
	///     data context to inner operators.
	/// </param>
	/// <returns>The result of the rule.</returns>
	public override JsonNode? Apply(JsonNode? data, JsonNode? contextData = null)
	{
		var items = Items.Select(i => i.Apply(data, contextData)).SelectMany(e => e.Flatten());

		return items.ToJsonArray();
	}

	/// <summary>
	/// Returns the TypeInfo that can serialize this Rule type.
	/// </summary>
	public override JsonTypeInfo TypeInfo => JsonLogicSerializerContext.Default.MergeRule;
}

internal class MergeRuleJsonConverter : JsonConverter<MergeRule>
{
	public override MergeRule? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		var node = JsonSerializer.Deserialize(ref reader, JsonLogicSerializerContext.Default.JsonNode);

		var parameters = node is JsonArray
			? node.Deserialize(JsonLogicSerializerContext.Default.RuleArray)
			: new[] { node.Deserialize(JsonLogicSerializerContext.Default.Rule)! };


		if (parameters == null) return new MergeRule();

		return new MergeRule(parameters);
	}

	public override void Write(Utf8JsonWriter writer, MergeRule value, JsonSerializerOptions options)
	{
		writer.WriteStartObject();
		writer.WritePropertyName("merge");
		writer.WriteRules(value.Items, options, false);
		writer.WriteEndObject();
	}
}
