﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Json.More;

namespace Json.Logic.Rules;

/// <summary>
/// Handles the `reduce` operation.
/// </summary>
[Operator("reduce")]
[JsonConverter(typeof(ReduceRuleJsonConverter))]
public partial class ReduceRule : Rule
{
	private class Intermediary
	{
		public JsonNode? Current { get; set; }
		public JsonNode? Accumulator { get; set; }
	}

	[JsonSerializable(typeof(Intermediary))]
	private partial class IntermediarySerializerContext : JsonSerializerContext
	{

	}

	private static readonly JsonSerializerOptions _options = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

	/// <summary>
	/// A sequence of values to reduce.
	/// </summary>
	protected internal Rule Input { get; }
	/// <summary>
	/// The reduction to perform.
	/// </summary>
	protected internal Rule Rule { get; }
	/// <summary>
	/// The initial value to start the reduction. ie; the seed.
	/// </summary>
	protected internal Rule Initial { get; }

	/// <summary>
	/// Creates a new instance of <see cref="ReduceRule"/> when 'reduce' operator is detected within json logic.
	/// </summary>
	/// <param name="input">A sequence of values to reduce.</param>
	/// <param name="rule">The reduction to perform.</param>
	/// <param name="initial">The initial value to start the reduction. ie; the seed.</param>
	protected internal ReduceRule(Rule input, Rule rule, Rule initial)
	{
		Input = input;
		Rule = rule;
		Initial = initial;
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
		var accumulator = Initial.Apply(data, contextData);

		if (input is not JsonArray arr) return accumulator;

		foreach (var element in arr)
		{
			var intermediary = new Intermediary
			{
				Current = element,
				Accumulator = accumulator
			};
			var item = JsonSerializer.SerializeToNode(intermediary, IntermediarySerializerContext.Default.Intermediary);

			accumulator = Rule.Apply(data, item);

			if (accumulator == JsonNull.SignalNode) break;
		}

		return accumulator;
	}

	/// <summary>
	/// Returns the TypeInfo that can serialize this Rule type.
	/// </summary>
	public override JsonTypeInfo TypeInfo => JsonLogicSerializerContext.Default.ReduceRule;
}

internal class ReduceRuleJsonConverter : JsonConverter<ReduceRule>
{
	public override ReduceRule? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		var parameters = JsonSerializer.Deserialize(ref reader, JsonLogicSerializerContext.Default.RuleArray);

		if (parameters is not { Length: 3 })
			throw new JsonException("The reduce rule needs an array with 3 parameters.");

		return new ReduceRule(parameters[0], parameters[1], parameters[2]);
	}

	public override void Write(Utf8JsonWriter writer, ReduceRule value, JsonSerializerOptions options)
	{
		writer.WriteStartObject();
		writer.WritePropertyName("reduce");
		writer.WriteStartArray();
		writer.WriteRule(value.Input, options);
		writer.WriteRule(value.Rule, options);
		writer.WriteRule(value.Initial, options);
		writer.WriteEndArray();
		writer.WriteEndObject();
	}
}
