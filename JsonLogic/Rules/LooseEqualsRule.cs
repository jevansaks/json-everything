﻿using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Json.More;

namespace Json.Logic.Rules;

/// <summary>
/// Handles the `==` operation.
/// </summary>
[Operator("==")]
[JsonConverter(typeof(LooseEqualsRuleJsonConverter))]
public class LooseEqualsRule : Rule
{
	/// <summary>
	/// First value to compare.
	/// </summary>
	protected internal Rule A { get; }
	/// <summary>
	/// Second value to compare.
	/// </summary>
	protected internal Rule B { get; }

	/// <summary>
	/// Creates a new instance of <see cref="LooseEqualsRule"/> when '==' operator is detected within json logic.
	/// </summary>
	/// <param name="a">First value to compare.</param>
	/// <param name="b">Second value to compare.</param>
	protected internal LooseEqualsRule(Rule a, Rule b)
	{
		A = a;
		B = b;
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
		var a = A.Apply(data, contextData);
		var b = B.Apply(data, contextData);

		return a.LooseEquals(b);
	}
}

internal class LooseEqualsRuleJsonConverter : AotCompatibleJsonConverter<LooseEqualsRule>
{
	public override LooseEqualsRule? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		var readerCopy = reader;
		var node = options.Read(ref readerCopy, LogicSerializerContext.Default.JsonNode);

		var parameters = options.Read(ref reader, LogicSerializerContext.Default.RuleArray);

		if (parameters is not { Length: 2 })
			throw new JsonException("The == rule needs an array with 2 parameters.");

		return new LooseEqualsRule(parameters[0], parameters[1]);
	}

	public override void Write(Utf8JsonWriter writer, LooseEqualsRule value, JsonSerializerOptions options)
	{
		writer.WriteStartObject();
		writer.WritePropertyName("==");
		writer.WriteStartArray();
		writer.WriteRule(value.A, options);
		writer.WriteRule(value.B, options);
		writer.WriteEndArray();
		writer.WriteEndObject();
	}
}
