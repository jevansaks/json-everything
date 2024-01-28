﻿using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Json.More;

namespace Json.Logic.Rules;

/// <summary>
/// Handles the `>=` operation.
/// </summary>
[Operator(">=")]
[JsonConverter(typeof(MoreThanEqualRuleJsonConverter))]
public class MoreThanEqualRule : Rule
{
	/// <summary>
	/// The value to test.
	/// </summary>
	protected internal Rule A { get; }
	/// <summary>
	/// The boundary to test against.
	/// </summary>
	protected internal Rule B { get; }

	/// <summary>
	/// Creates a new instance of <see cref="MoreThanEqualRule"/> when '>=' operator is detected within json logic.
	/// </summary>
	/// <param name="a">The value to test.</param>
	/// <param name="b">The boundary to test against.</param>
	protected internal MoreThanEqualRule(Rule a, Rule b)
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

		if (a is JsonValue av && av.TryGetValue(out string? s1) &&
		    b is JsonValue bv && bv.TryGetValue(out string? s2))
			return string.Compare(s1, s2, StringComparison.Ordinal) >= 0;

		var numberA = a.Numberify();
		var numberB = b.Numberify();

		if (numberA != null && numberB != null) return numberA >= numberB;
		if (numberA != null || numberB != null) return false;

		var stringA = a.Stringify();
		var stringB = b.Stringify();

		return string.Compare(stringA, stringB, StringComparison.Ordinal) >= 0;
	}
}

internal class MoreThanEqualRuleJsonConverter : AotCompatibleJsonConverter<MoreThanEqualRule>
{
	public override MoreThanEqualRule? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		var parameters = options.Read<Rule[]>(ref reader);

		if (parameters is not { Length: 2 })
			throw new JsonException("The >= rule needs an array with 2 parameters.");

		return new MoreThanEqualRule(parameters[0], parameters[1]);
	}

	public override void Write(Utf8JsonWriter writer, MoreThanEqualRule value, JsonSerializerOptions options)
	{
		writer.WriteStartObject();
		writer.WritePropertyName(">=");
		writer.WriteStartArray();
		writer.WriteRule(value.A, options);
		writer.WriteRule(value.B, options);
		writer.WriteEndArray();
		writer.WriteEndObject();
	}
}
