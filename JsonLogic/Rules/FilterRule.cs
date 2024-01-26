﻿using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Json.More;

namespace Json.Logic.Rules;

/// <summary>
/// Handles the `filter` operation.
/// </summary>
[Operator("filter")]
[JsonConverter(typeof(FilterRuleJsonConverter))]
public class FilterRule : Rule
{
	/// <summary>
	/// A sequence of values to filter.
	/// </summary>
	protected internal Rule Input { get; }
	
	/// <summary>
	/// A predicate to apply to each item in the sequence.
	/// </summary>
	protected internal Rule Rule { get; }

	/// <summary>
	/// Creates a new instance of <see cref="FilterRule"/> when 'filter' operator is detected within json logic.
	/// </summary>
	/// <param name="input">A sequence of values to filter.</param>
	/// <param name="rule">A predicate to apply to each item in the sequence.</param>
	protected internal FilterRule(Rule input, Rule rule)
	{
		Input = input;
		Rule = rule;
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
		var input = Input.Apply(data, contextData);

		if (input is not JsonArray arr)
			return new JsonArray();

		return arr.Where(i => Rule.Apply(data, i).IsTruthy()).ToJsonArray();
	}

	/// <summary>
	/// Returns the TypeInfo that can serialize this Rule type.
	/// </summary>
	public override JsonTypeInfo TypeInfo => JsonLogicSerializerContext.Default.FilterRule;
}

internal class FilterRuleJsonConverter : JsonConverter<FilterRule>
{
	public override FilterRule? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		var parameters = JsonSerializer.Deserialize(ref reader, JsonLogicSerializerContext.Default.RuleArray);

		if (parameters is not { Length: 2 })
			throw new JsonException("The filter rule needs an array with 2 parameters.");

		return new FilterRule(parameters[0], parameters[1]);
	}

	public override void Write(Utf8JsonWriter writer, FilterRule value, JsonSerializerOptions options)
	{
		writer.WriteStartObject();
		writer.WritePropertyName("filter");
		writer.WriteStartArray();
		writer.WriteRule(value.Input, options);
		writer.WriteRule(value.Rule, options);
		writer.WriteEndArray();
		writer.WriteEndObject();
	}
}
