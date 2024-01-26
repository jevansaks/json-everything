﻿using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Json.Pointer;

namespace Json.Logic.Rules;

/// <summary>
/// Handles the `var` operation.
/// </summary>
[Operator("var")]
[JsonConverter(typeof(VariableRuleJsonConverter))]
public class VariableRule : Rule
{
	internal Rule? Path { get; }
	internal Rule? DefaultValue { get; }

	internal VariableRule()
	{
	}
	internal VariableRule(Rule path)
	{
		Path = path;
	}
	internal VariableRule(Rule path, Rule defaultValue)
	{
		Path = path;
		DefaultValue = defaultValue;
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
		if (Path == null) return data;

		var path = Path.Apply(data, contextData);
		var pathString = path.Stringify()!;
		if (pathString == string.Empty) return contextData ?? data;

		var pointer = JsonPointer.Parse(pathString == string.Empty ? "" : $"/{pathString.Replace('.', '/')}");
		if (pointer.TryEvaluate(contextData ?? data, out var pathEval) ||
			pointer.TryEvaluate(data, out pathEval))
			return pathEval;

		return DefaultValue?.Apply(data, contextData) ?? null;
	}

	/// <summary>
	/// Returns the TypeInfo that can serialize this Rule type.
	/// </summary>
	public override JsonTypeInfo TypeInfo => JsonLogicSerializerContext.Default.VariableRule;
}

internal class VariableRuleJsonConverter : JsonConverter<VariableRule>
{
	public override VariableRule? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		var node = JsonSerializer.Deserialize(ref reader, JsonLogicSerializerContext.Default.JsonNode);

		var parameters = node is JsonArray
			? node.Deserialize(JsonLogicSerializerContext.Default.RuleArray)
			: new[] { node.Deserialize<Rule>(JsonLogicSerializerContext.Default.Rule)! };

		if (parameters is not ({ Length: 0 } or { Length: 1 } or { Length: 2 }))
			throw new JsonException("The var rule needs an array with 0, 1, or 2 parameters.");

		return parameters.Length switch
		{
			0 => new VariableRule(),
			1 => new VariableRule(parameters[0]),
			_ => new VariableRule(parameters[0], parameters[1])
		};
	}

	public override void Write(Utf8JsonWriter writer, VariableRule value, JsonSerializerOptions options)
	{
		writer.WriteStartObject();
		writer.WritePropertyName("var");
		if (value.DefaultValue != null)
		{
			writer.WriteStartArray();
			writer.WriteRule(value.Path, options);
			writer.WriteRule(value.DefaultValue, options);
			writer.WriteEndArray();
		}
		else
			writer.WriteRule(value.Path, options);

		writer.WriteEndObject();
	}
}
