﻿using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Json.More;

namespace Json.Schema;

/// <summary>
/// Handles unrecognized keywords.
/// </summary>
[SchemaSpecVersion(SpecVersion.Draft6)]
[SchemaSpecVersion(SpecVersion.Draft7)]
[SchemaSpecVersion(SpecVersion.Draft201909)]
[SchemaSpecVersion(SpecVersion.Draft202012)]
[SchemaSpecVersion(SpecVersion.DraftNext)]
[JsonConverter(typeof(UnrecognizedKeywordJsonConverter))]
public class UnrecognizedKeyword : IJsonSchemaKeyword
{
	/// <summary>
	/// The name or key of the keyword.
	/// </summary>
	public string Name { get; }

	/// <summary>
	/// The value of the keyword.
	/// </summary>
	public JsonNode? Value { get; }

	/// <summary>
	/// Creates a new <see cref="UnrecognizedKeyword"/>.
	/// </summary>
	/// <param name="name">The name of the keyword.</param>
	/// <param name="value">The value of the keyword.</param>
	public UnrecognizedKeyword(string name, JsonNode? value)
	{
		Name = name;
		Value = value;
	}

	public KeywordConstraint GetConstraint(SchemaConstraint schemaConstraint,
		IReadOnlyList<KeywordConstraint> localConstraints,
		ConstraintBuilderContext context)
	{
		return new KeywordConstraint(Name, (e, _) => e.Results.SetAnnotation(Name, Value));
	}
}

internal class UnrecognizedKeywordJsonConverter : JsonConverter<UnrecognizedKeyword>
{
	public override UnrecognizedKeyword Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		throw new NotImplementedException("Unrecognized keywords should be handled manually during JsonSchema deserialization.");
	}

	public override void Write(Utf8JsonWriter writer, UnrecognizedKeyword value, JsonSerializerOptions options)
	{
		writer.WritePropertyName(value.Name);
		JsonSerializer.Serialize(writer, value.Value, options);
	}
}