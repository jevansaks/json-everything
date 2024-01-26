using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Json.Logic;

/// <summary>
/// Provides extended functionality for serializing rules.
/// </summary>
public static class JsonWriterExtensions
{
	/// <summary>
	/// Writes a rule to the stream, taking its specific type into account.
	/// </summary>
	/// <param name="writer">The writer.</param>
	/// <param name="rule">The rule.</param>
	/// <param name="options">Serializer options.</param>
	public static void WriteRule(this Utf8JsonWriter writer, Rule? rule, JsonSerializerOptions options)
	{
		if (rule == null)
		{
			writer.WriteNullValue();
			return;
		}
#if NET6_0_OR_GREATER
		JsonSerializer.Serialize(writer, rule, rule.TypeInfo);
#else
		JsonSerializer.Serialize(writer, rule, rule.GetType(), options);
#endif
	}

	/// <summary>
	/// Writes a rule to the stream, taking its specific type into account.
	/// </summary>
	/// <param name="writer">The writer.</param>
	/// <param name="rules">The rules.</param>
	/// <param name="options">Serializer options.</param>
	/// <param name="unwrapSingle">Unwraps single items instead of writing an array.</param>
	public static void WriteRules(this Utf8JsonWriter writer, IEnumerable<Rule> rules, JsonSerializerOptions options, bool unwrapSingle = true)
	{
		var array = rules.ToArray();
		if (unwrapSingle && array.Length == 1)
		{
			writer.WriteRule(array[0], options);
			return;
		}

		writer.WriteStartArray();
		foreach (var rule in array)
		{
			writer.WriteRule(rule, options);
		}
		writer.WriteEndArray();
	}
}